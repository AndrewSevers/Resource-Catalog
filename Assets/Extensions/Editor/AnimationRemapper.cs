using Extensions.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Resource.Editor {

    public class AnimationRemapper : EditorWindow {
        private Animator animator = null;
        private List<AnimationClipInfo> clips = null;

        private int clipIndex = 0;
        private int pendingChanges = 0;
        private bool refresh = false;

        private Color defaultColor;
        private Vector2 scrollPosition = Vector2.zero;
        private string error = null;

        private const string upArrowUnicode = "\u25b2";
        private const string downArrowUnicode = "\u25bc";

        [MenuItem("Window/Animation Remapper")]
        private static void ShowAnimationHierarchyWindow() {
            AnimationRemapper window = GetWindow<AnimationRemapper>(true, "Animation Remapper", true);
            window.SetupAnimator(Selection.activeGameObject);
            window.Initialize();
        }

        [MenuItem("GameObject/Remap Animation", false, 48)]
        private static void ModifyAnimationHierarchy() {
            AnimationRemapper window = GetWindow<AnimationRemapper>(true, "Animation Remapper", true);
            window.SetupAnimator(Selection.activeGameObject);
            window.Initialize();
        }

        #region Animation Clip Reference Class
        public class AnimationClipInfo {
            private AnimationClip clip;
            private List<EditorBindingInfo> bindings;

            #region Getters & Setters
            public AnimationClip Clip {
                get { return clip; }
            }

            public List<EditorBindingInfo> Bindings {
                get { return bindings; }
                set { bindings = value; }
            }
            #endregion

            #region Constructor
            public AnimationClipInfo(AnimationClip aClip, List<EditorBindingInfo> aBindings = null) {
                clip = aClip;
                bindings = aBindings;
            }
            #endregion
        }
        #endregion

        #region Editor Binding Info Class
        public class EditorBindingInfo {
            private AnimationClipInfo clipInfo;
            private string path;
            private string newPath;
            private List<EditorCurveBinding> bindings;                                              // Bindings for the current animation clip
            private List<EditorBindingInfo> associatedBindings = new List<EditorBindingInfo>();     // Matching bindings from other animation clips that have the same path
            private bool hasPendingChanges = false;
            private bool showAssociatedBindings = false;

            #region Getters & Setters
            public AnimationClipInfo ClipInfo {
                get { return clipInfo; }
            }

            public string Path {
                get { return path; }
            }

            public string NewPath {
                get { return newPath; }
            }

            public List<EditorCurveBinding> Bindings {
                get { return bindings; }
                set { bindings = value; }
            }

            public List<EditorBindingInfo> AssociatedBindings {
                get { return associatedBindings; }
                set { associatedBindings = value; }
            }

            public bool HasPendingChanges {
                get { return hasPendingChanges; }
            }

            public bool ShowAssociatedBindings {
                get { return showAssociatedBindings; }
                set { showAssociatedBindings = value; }
            }
            #endregion

            #region Contructor
            public EditorBindingInfo(AnimationClipInfo aClipInfo, string aPath, List<EditorCurveBinding> aBindings) {
                clipInfo = aClipInfo;
                path = aPath;
                bindings = aBindings;
            }
            #endregion

            #region Utilities
            public void UpdatePath() {
                path = newPath;

                newPath = null;
                hasPendingChanges = false;
                showAssociatedBindings = false;
            }

            public void SetPendingUpdate(string aNewPath) {
                newPath = aNewPath;
                hasPendingChanges = true;
            }

            public void ClearPendingChanges() {
                newPath = null;
                hasPendingChanges = false;
                showAssociatedBindings = false;
            }
            #endregion

        }
        #endregion

        #region Initialize
        private void Initialize() {
            defaultColor = GUI.color;
        }
        #endregion

        #region OnGUI
        private void OnGUI() {
            GUILayout.BeginHorizontal();

            Animator currentAnimator = animator;
            animator = EditorGUILayout.ObjectField(animator, typeof(Animator), true) as Animator;

            if (currentAnimator != animator) {
                refresh = true;
            }

            // Draw selected animation clips bindings
            if (clips != null && clips.Count > 0) {
                int originalClipIndex = clipIndex;
                clipIndex = EditorGUILayout.Popup(clipIndex, clips.Select(c => c.Clip.name).ToArray());

                GUILayout.EndHorizontal();

                // Draw Separator
                GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

                // Draw bindings for the selected animation
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                foreach (EditorBindingInfo info in clips[clipIndex].Bindings) {
                    DrawObjectReference(info, originalClipIndex != clipIndex);
                }
                GUILayout.EndScrollView();
            } else {
                // Keep refreshing checking to see if any clips are added
                refresh = true;

                GUILayout.EndHorizontal();
            }

            // Draw and handle buttons that affect all pending changes
            bool applyAllPendingChanges = false;
            bool clearAllPendingChanges = false;
            if (pendingChanges > 0) {
                // Draw Separator
                GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply All Pending Changes")) {
                    applyAllPendingChanges = true;
                }

                if (GUILayout.Button("Clear All Pending Changes")) {
                    clearAllPendingChanges = true;
                }
                GUILayout.EndHorizontal();
            }

            // Display an error
            if (string.IsNullOrEmpty(error) == false) {
                DrawError();
            }

            if (refresh) {
                refresh = false;

                pendingChanges = 0;

                SetupAnimationClips();
            } else if (applyAllPendingChanges) {
                // Apply all pending changes across all animations on the selected animator
                foreach (AnimationClipInfo clipInfo in clips) {
                    foreach (EditorBindingInfo bindingInfo in clipInfo.Bindings) {
                        if (bindingInfo.HasPendingChanges) {
                            ApplyPendingChanges(bindingInfo);
                        }
                    }
                }
            } else if (clearAllPendingChanges) {
                // Clear all pending changes across all animations
                foreach (AnimationClipInfo clipInfo in clips) {
                    foreach (EditorBindingInfo bindingInfo in clipInfo.Bindings) {
                        if (bindingInfo.HasPendingChanges) {
                            ClearPendingChanges(bindingInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the object reference's binding info (path and link to object)
        /// </summary>
        /// <param name="aClipInfo">AnimationClipInfo associated to the BindingInfo to draw</param>
        /// <param name="aBindingInfo">BindingInfo to draw</param>
        private void DrawObjectReference(EditorBindingInfo aBindingInfo, bool aIsInitialDraw) {
            GameObject objectReference = FindObjectInRoot(aBindingInfo.Path);

            // Cleanup references when drawing them for the first time (or switching which animation clip is being drawn)
            if (aIsInitialDraw) {
                aBindingInfo.ShowAssociatedBindings = false;
                
            }

            if (aBindingInfo.HasPendingChanges) {
                GUI.color = Color.yellow;
            } else if (objectReference != null) {
                GUI.color = Color.green;
            } else {
                GUI.color = Color.red;
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.color = defaultColor;
            
            GUIContent label = new GUIContent(" Path: /" + aBindingInfo.Path, "/" + aBindingInfo.Path);
            GUILayout.Label(label, GUILayout.Width(150.0f));

            if (aBindingInfo.HasPendingChanges) {
                objectReference = FindObjectInRoot(aBindingInfo.NewPath);
            }

            // Draw the object field with the latest information and color it based on status. Red = invalid object (null), Yellow = pending change, Green = valid object
            GameObject newObjectReference = EditorGUILayout.ObjectField(objectReference, typeof(GameObject), true) as GameObject;

            // Update binding info's pending change data with new object reference
            if (newObjectReference != objectReference) {
                if (aBindingInfo.HasPendingChanges == false) {
                    pendingChanges++;
                }

                aBindingInfo.SetPendingUpdate(GetPath(newObjectReference));
            }

            GUI.enabled = aBindingInfo.HasPendingChanges;

            if (GUILayout.Button("Apply", EditorStyles.miniButtonLeft)) {
                // Setup list of bindings to update base on the curently selected one as well as the current one's associated bindings
                List<EditorBindingInfo> bindings = new List<EditorBindingInfo>() { aBindingInfo };
                foreach (EditorBindingInfo associatedBindingInfo in aBindingInfo.AssociatedBindings) {
                    if (associatedBindingInfo.HasPendingChanges && associatedBindingInfo.NewPath.Equals(aBindingInfo.NewPath)) {
                        bindings.Add(associatedBindingInfo);
                    }
                }

                ApplyPendingChanges(bindings.ToArray());
            }

            if (GUILayout.Button("Clear", EditorStyles.miniButtonMid)) {
                ClearPendingChanges(aBindingInfo);
            }

            if (aBindingInfo.AssociatedBindings.Count > 0) {
                GUI.enabled = true;
            }

            // Diplay the show associated bindings button
            string arrowUnicode = (aBindingInfo.ShowAssociatedBindings) ? upArrowUnicode : downArrowUnicode;
            GUIContent expandContent = new GUIContent(arrowUnicode, "Expand to show associated bindings among the other animation clips on this animator");

            if (GUILayout.Button(expandContent, EditorStyles.miniButtonRight)) {
                aBindingInfo.ShowAssociatedBindings = !aBindingInfo.ShowAssociatedBindings;
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            // Draw associated bindings
            if (aBindingInfo.ShowAssociatedBindings) {
                if (aBindingInfo.HasPendingChanges == false) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" Can't modify associated bindings if the current one is not being modified.", EditorStyles.miniLabel);
                    GUILayout.EndHorizontal();
                } else {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" List of animations with this same binding. Check to associate changes across animations.", EditorStyles.miniLabel);
                    GUILayout.EndHorizontal();

                    foreach (EditorBindingInfo associatedBindingInfo in aBindingInfo.AssociatedBindings) {
                        DrawAssociatedObjectReference(aBindingInfo, associatedBindingInfo);
                    }
                }
            }
        }

        private void DrawAssociatedObjectReference(EditorBindingInfo aCurrentBindingInfo, EditorBindingInfo aAssociatedBindingInfo) {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.25f));
            GUIContent label = new GUIContent(" Clip - [" + aAssociatedBindingInfo.ClipInfo.Clip.name + "]", aAssociatedBindingInfo.ClipInfo.Clip.name);
            GUILayout.Label(label);
            
            GUILayout.FlexibleSpace();

            if (aAssociatedBindingInfo.HasPendingChanges && aAssociatedBindingInfo.NewPath.Equals(aCurrentBindingInfo.NewPath) == false) {
                GUILayout.Label("Clip already has different pending changes", EditorStyles.miniLabel);
            } else {
                bool set = GUILayout.Toggle(aAssociatedBindingInfo.HasPendingChanges, string.Empty);
                if (set) {
                    if (aAssociatedBindingInfo.HasPendingChanges == false) {
                        aAssociatedBindingInfo.SetPendingUpdate(aCurrentBindingInfo.NewPath);
                        pendingChanges++;
                    }
                } else {
                    if (aAssociatedBindingInfo.HasPendingChanges) {
                        aAssociatedBindingInfo.ClearPendingChanges();
                        pendingChanges--;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        private void DrawError() {
            GUI.color = Color.red;
            GUILayout.Label(error, EditorStyles.whiteLargeLabel);
            GUI.color = defaultColor;
        }
        #endregion

        #region Animator/Clips Setup
        /// <summary>
        /// Setup the active animator by caching the reference
        /// </summary>
        /// <param name="aAnimatorObject">Animator reference to cache</param>
        /// <param name="aSetupAnimationClips">If true also cache the animation clip data</param>
        private void SetupAnimator(GameObject aAnimatorObject = null, bool aSetupAnimationClips = true) {
            if (aAnimatorObject != null) {
                animator = aAnimatorObject.GetComponent<Animator>();
            }

            if (aSetupAnimationClips) {
                SetupAnimationClips();
            }
        }

        /// <summary>
        /// Cache all the animation clip data for the active animator
        /// </summary>
        private void SetupAnimationClips() {
            if (animator != null) {
                if (animator.runtimeAnimatorController != null) {
                    if (animator.runtimeAnimatorController.animationClips.Length > 0) {
                        clips = new List<AnimationClipInfo>();

                        // Setup animation clip bindings
                        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
                            AnimationClipInfo info = new AnimationClipInfo(clip);
                            info.Bindings = SetupAnimationClipBindings(info);

                            clips.Add(info);
                        }

                        // Setup associative connections
                        SetupAssociativeBindings();
                    } else {
                        error = "Animator contains no animation clips";
                    }
                } else {
                    error = "Animator contains no runtime controller";
                }
            }
        }

        /// <summary>
        /// Multiple animation clips can have the same EditorCurveBindings so connect those
        /// </summary>
        private void SetupAssociativeBindings(bool aRefreshAssociations = false) {
            foreach (AnimationClipInfo clipInfo in clips) {
                foreach (EditorBindingInfo bindingInfo in clipInfo.Bindings) {
                    if (aRefreshAssociations) {
                        bindingInfo.AssociatedBindings.Clear();
                    }

                    foreach (AnimationClipInfo clipInfo2 in clips) {
                        if (clipInfo == clipInfo2) {
                            continue;
                        }

                        foreach (EditorBindingInfo bindingInfo2 in clipInfo2.Bindings) {
                            if (bindingInfo.Path.Equals(bindingInfo2.Path) == false) {
                                continue;
                            }

                            bindingInfo.AssociatedBindings.Add(bindingInfo2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cache animaiton clip bindings. There can be multiple bindings to a single path (object) since each animation property is a new binding.
        /// </summary>
        /// <param name="aClipInfo">Clip to generate a list of bindings for</param>
        private List<EditorBindingInfo> SetupAnimationClipBindings(AnimationClipInfo aClipInfo) {
            // MultiDictionary holds a single key to many values
            MultiDictionary<string, EditorCurveBinding> bindingsMap = new MultiDictionary<string, EditorCurveBinding>();
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(aClipInfo.Clip)) {
                bindingsMap.Add(binding.path, binding);
            }

            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(aClipInfo.Clip)) {
                bindingsMap.Add(binding.path, binding);
            }

            // Convert the MultiDictionary to a list of modifiable and maintainable objects (this allows us to support "pending" changes)
            List<EditorBindingInfo> bindings = new List<EditorBindingInfo>();
            foreach (string key in bindingsMap.Keys) {
                bindings.Add(new EditorBindingInfo(aClipInfo, key, bindingsMap[key]));
            }

            return bindings;
        }
        #endregion

        #region Object References Modifications
        /// <summary>
        /// Apply changes to the provided BindingInfo by updating the animations object bindings
        /// </summary>
        /// <param name="aClipInfo">AnimationClipInfo associated to the BindingInfo</param>
        /// <param name="aBindingInfo">BindingInfo to update</param>
        private void ApplyPendingChanges(params EditorBindingInfo[] aBindingInfos) {
            EditorUtility.DisplayProgressBar("Processing", "Remapping animation bindings!", 0.0f);
            float progressIteration = (100.0f / (float) aBindingInfos.Length);
            float progress = 0;

            AssetDatabase.StartAssetEditing();

            foreach (EditorBindingInfo info in aBindingInfos) {
                foreach (EditorCurveBinding binding in info.Bindings) {
                    UpdateBinding(info, binding);

                    progress += progressIteration;
                    EditorUtility.DisplayProgressBar("Processing", "Remapping animation bindings!", progress);
                }

                pendingChanges--;

                info.UpdatePath();
            }

            // Refresh associative bindings now that paths have changes
            SetupAssociativeBindings(true);

            AssetDatabase.StopAssetEditing();

            Repaint();

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Clear all pending changes from provided BindingInfo
        /// </summary>
        /// <param name="aBindingInfo">BindingInfo to update</param>
        private void ClearPendingChanges(EditorBindingInfo aBindingInfo) {
            aBindingInfo.ClearPendingChanges();
            pendingChanges--;
        }
        #endregion

        #region Utilities
        private void UpdateBinding(EditorBindingInfo aBindingInfo, EditorCurveBinding aBinding) {
            EditorCurveBinding bindingToUpdate = aBinding;
            AnimationCurve curve = AnimationUtility.GetEditorCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate);

            if (curve != null) {
                AnimationUtility.SetEditorCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate, null);
            } else {
                AnimationUtility.SetObjectReferenceCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate, null);
            }

            bindingToUpdate.path = aBindingInfo.NewPath;

            if (curve != null) {
                AnimationUtility.SetEditorCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate, curve);
            } else {
                ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate);
                AnimationUtility.SetObjectReferenceCurve(aBindingInfo.ClipInfo.Clip, bindingToUpdate, objectReferenceCurve);
            }
        }

        /// <summary>
        /// Get the absolute path for the object (recursive function)
        /// </summary>
        /// <param name="aObjectReference">Object to find the path for</param>
        /// <param name="aSeparatePath">If true add separator to the path</param>
        private string GetPath(GameObject aObjectReference, bool aSeparatePath = false) {
            if (aObjectReference != animator.gameObject) {
                if (aObjectReference.transform.parent != null) {
                    return GetPath(aObjectReference.transform.parent.gameObject, true) + aObjectReference.name + (aSeparatePath ? "/" : string.Empty);
                } else {
                    throw new UnityException(string.Format("Object must belong to animator '{0}'!", animator));
                }
            } else {
                return string.Empty;
            }
        }

        private GameObject FindObjectInRoot(string aPath) {
            Transform child = animator.transform.Find(aPath);
            return (child != null) ? child.gameObject : null;
        }
        #endregion

    }

}
