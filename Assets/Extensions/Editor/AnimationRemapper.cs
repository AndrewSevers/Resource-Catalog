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
            private string path;
            private string newPath;
            private List<EditorCurveBinding> bindings;
            private bool hasPendingChanges = false;

            #region Getters & Setters
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

            public bool HasPendingChanges {
                get { return hasPendingChanges; }
            }
            #endregion

            #region Contructor
            public EditorBindingInfo(string aPath, List<EditorCurveBinding> aBindings) {
                path = aPath;
                bindings = aBindings;
            }
            #endregion

            #region Utilities
            public void UpdatePath() {
                path = newPath;

                newPath = null;
                hasPendingChanges = false;
            }

            public void SetPendingUpdate(string aNewPath) {
                newPath = aNewPath;
                hasPendingChanges = true;
            }

            public void ClearPendingChanges() {
                newPath = null;
                hasPendingChanges = false;
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
                clipIndex = EditorGUILayout.Popup(clipIndex, clips.Select(c => c.Clip.name).ToArray());

                GUILayout.EndHorizontal();

                // Draw Separator
                GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

                // Draw bindings for the selected animation
                GUILayout.BeginScrollView(scrollPosition);
                foreach (EditorBindingInfo info in clips[clipIndex].Bindings) {
                    DrawObjectReference(clips[clipIndex], info);
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
                            ApplyPendingChanges(clipInfo, bindingInfo);
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
        private void DrawObjectReference(AnimationClipInfo aClipInfo, EditorBindingInfo aBindingInfo) {
            GameObject objectReference = FindObjectInRoot(aBindingInfo.Path);

            GUILayout.BeginHorizontal();

            GUIContent label = new GUIContent("Path: /" + aBindingInfo.Path, "/" + aBindingInfo.Path);
            GUILayout.Label(label, GUILayout.Width(150.0f));

            if (aBindingInfo.HasPendingChanges) {
                objectReference = FindObjectInRoot(aBindingInfo.NewPath);
            }

            // Draw the object field with the latest information and color it based on status. Red = invalid object (null), Yellow = pending change, Green = valid object
            GUI.color = (objectReference != null) ? (aBindingInfo.HasPendingChanges) ? Color.yellow : Color.green : Color.red;
            GameObject newObjectReference = EditorGUILayout.ObjectField(objectReference, typeof(GameObject), true) as GameObject;
            GUI.color = defaultColor;

            // Update binding info's pending change data with new object reference
            if (newObjectReference != objectReference) {
                if (aBindingInfo.HasPendingChanges == false) {
                    pendingChanges++;
                }

                aBindingInfo.SetPendingUpdate(GetPath(newObjectReference));
            }

            GUI.enabled = aBindingInfo.HasPendingChanges;

            if (GUILayout.Button("Apply", EditorStyles.miniButton)) {
                ApplyPendingChanges(aClipInfo, aBindingInfo);
            }

            if (GUILayout.Button("Clear", EditorStyles.miniButton)) {
                ClearPendingChanges(aBindingInfo);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }
        
        private void DrawError() {
            GUI.color = Color.red;
            GUILayout.Label(error, EditorStyles.whiteLargeLabel);
            GUI.color = defaultColor;
        }
        #endregion

        #region Animator
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
                if (clips == null) {
                    clips = new List<AnimationClipInfo>();
                }

                if (animator.runtimeAnimatorController != null) {
                    if (animator.runtimeAnimatorController.animationClips.Length > 0) {
                        clips = new List<AnimationClipInfo>();

                        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
                            clips.Add(new AnimationClipInfo(clip, SetupAnimationClipBindings(clip)));
                        }
                    } else {
                        error = "Animator contains no animation clips";
                    }
                } else {
                    error = "Animator contains no runtime controller";
                }

            }
        }

        /// <summary>
        /// Cache animaiton clip bindings. There can be multiple bindings to a single path (object) since each animation property is a new binding.
        /// </summary>
        /// <param name="aClip">Clip to generate a list of bindings for</param>
        private List<EditorBindingInfo> SetupAnimationClipBindings(AnimationClip aClip) {
            // MultiDictionary holds a single key to many values
            MultiDictionary<string, EditorCurveBinding> bindingsMap = new MultiDictionary<string, EditorCurveBinding>();
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(aClip)) {
                bindingsMap.Add(binding.path, binding);
            }

            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(aClip)) {
                bindingsMap.Add(binding.path, binding);
            }

            // Convert the MultiDictionary to a list of modifiable and maintainable objects (this allows us to support "pending" changes)
            List<EditorBindingInfo> bindings = new List<EditorBindingInfo>();
            foreach (string key in bindingsMap.Keys) {
                bindings.Add(new EditorBindingInfo(key, bindingsMap[key]));
            }

            return bindings;
        }
        #endregion

        #region Object References
        /// <summary>
        /// Apply changes to the provided BindingInfo by updating the animations object bindings
        /// </summary>
        /// <param name="aClipInfo">AnimationClipInfo associated to the BindingInfo</param>
        /// <param name="aBindingInfo">BindingInfo to update</param>
        private void ApplyPendingChanges(AnimationClipInfo aClipInfo, EditorBindingInfo aBindingInfo) {
            EditorUtility.DisplayProgressBar("Processing", "Remapping animation bindings!", 0.0f);
            float progressIteration = (100.0f / (float) aBindingInfo.Bindings.Count);
            float progress = 0;

            AssetDatabase.StartAssetEditing();

            foreach (EditorCurveBinding binding in aBindingInfo.Bindings) {
                EditorCurveBinding bindingToUpdate = binding;
                AnimationCurve curve = AnimationUtility.GetEditorCurve(aClipInfo.Clip, bindingToUpdate);

                if (curve != null) {
                    AnimationUtility.SetEditorCurve(aClipInfo.Clip, bindingToUpdate, null);
                } else {
                    AnimationUtility.SetObjectReferenceCurve(aClipInfo.Clip, bindingToUpdate, null);
                }

                bindingToUpdate.path = aBindingInfo.NewPath;

                if (curve != null) {
                    AnimationUtility.SetEditorCurve(aClipInfo.Clip, bindingToUpdate, curve);
                } else {
                    ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(aClipInfo.Clip, bindingToUpdate);
                    AnimationUtility.SetObjectReferenceCurve(aClipInfo.Clip, bindingToUpdate, objectReferenceCurve);
                }

                progress += progressIteration;
                EditorUtility.DisplayProgressBar("Processing", "Remapping animation bindings!", progress);
            }

            AssetDatabase.StopAssetEditing();

            pendingChanges--;

            aBindingInfo.UpdatePath();

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
