using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Resource.Editor {

    public class PrefabTools : EditorWindow {
        private Vector2 scrollPosition = Vector2.zero;
        private List<Reference> references = new List<Reference>();

        [MenuItem("GameObject/Manage Selected Prefabs %#m", false, 101)]
        private static void ShowManagePrefabsWindow() {
            PrefabTools window = GetWindow<PrefabTools>(true, "Manage Prefabs", true);
            window.FindPrefabsInSelection(Selection.gameObjects);
        }

        [MenuItem("GameObject/Manage Selected Prefabs %#m", true)]
        private static bool ValidatePrefabStatus() {
            bool isPrefab = (PrefabUtility.GetPrefabObject(Selection.activeGameObject) != null);

            if (isPrefab == false) {
                Debug.Log("[PrefabTools] GameObject is not a prefab instance!");
            }

            return isPrefab;
        }

        #region Reference Calss
        private class Reference {
            private GameObject prefab;
            private bool selected;

            #region Getters & Setters
            public GameObject Prefab {
                get { return prefab; }
            }

            public bool Selected {
                get { return selected; }
                set { selected = value; }
            }
            #endregion

            #region Constructor
            public Reference(GameObject aPrefab, bool aSelected = false) {
                prefab = aPrefab;
                selected = aSelected;
            }
            #endregion
        }
        #endregion

        #region OnGUI
        private void OnGUI() {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected: " + references.FindAll(r => r.Selected).Count);
            if (GUILayout.Button("Select All", EditorStyles.miniButton)) {
                foreach (Reference aPrefab in references) {
                    UpdateSelection(aPrefab, true);
                }
            }

            if (GUILayout.Button("Deselect All", EditorStyles.miniButton)) {
                foreach (Reference aPrefab in references) {
                    UpdateSelection(aPrefab, false);
                }
            }
            GUILayout.EndHorizontal();

            // Draw Separator
            GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = references.Count - 1; i >= 0; i--) {
                LayoutItem(references[i]);
            }
            EditorGUILayout.EndScrollView();

            // Draw Separator
            GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

            GUILayout.BeginVertical();
            if (GUILayout.Button("Apply All Selected", EditorStyles.miniButton)) {
                ApplySelectedPrefabs();
            }

            if (GUILayout.Button("Revert All Selected", EditorStyles.miniButton)) {
                RevertSelectedPrefabs();
            }

            GUILayout.EndVertical();
            GUILayout.Space(5);
        }

        /// <summary>Layout item within the window</summary>
        private void LayoutItem(Reference aReference) {
            GUIStyle style = EditorStyles.miniButtonLeft;
            style.alignment = TextAnchor.MiddleLeft;

            GUILayout.BeginHorizontal();

            Color original = GUI.backgroundColor;
            if (aReference.Selected) {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(aReference.Prefab.name)) {
                    aReference.Selected = false;
                }
            } else {
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button(aReference.Prefab.name)) {
                    aReference.Selected = true;
                }
            }
            GUI.backgroundColor = original;

            GUILayout.EndHorizontal();
        }
        #endregion

        #region Search
        // Look for connections
        private void FindPrefabsInSelection(GameObject[] aSelection) {
            EditorUtility.DisplayProgressBar("Searching", "Finding all prefabs in selection", 0.0f);

            int updateIteration = Mathf.Max(1, aSelection.Length / 100);
            float progress = 0;

            if (aSelection.Length > 0) {
                for (int i = 0; i < aSelection.Length; i++) {
                    bool isPrefab = true;

                    GameObject currentGameObject = aSelection[i];
                    PrefabType prefabType = PrefabUtility.GetPrefabType(currentGameObject);

                    // Ensure the gameobject is a prefab
                    if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance) {
                        GameObject prefabRoot = ((GameObject) PrefabUtility.GetPrefabParent(currentGameObject)).transform.root.gameObject;
                        bool isTopOfHierarchy = false;

                        // Find the root of the 
                        while (currentGameObject.transform.parent != null && isTopOfHierarchy == false) {
                            // Are we still in the same prefab?
                            if (prefabRoot == ((GameObject) PrefabUtility.GetPrefabParent(currentGameObject.transform.parent.gameObject)).transform.root.gameObject) {
                                currentGameObject = currentGameObject.transform.parent.gameObject;
                            } else {
                                // The gameobject parent is another prefab, we stop here
                                isTopOfHierarchy = true;
                                if (prefabRoot != ((GameObject) PrefabUtility.GetPrefabParent(currentGameObject))) {
                                    isPrefab = false;
                                }
                            }
                        }

                        if (isPrefab) {
                            references.Add(new Reference(currentGameObject));
                        }

                        if (i % updateIteration == 0) {
                            progress += 0.01f;
                            EditorUtility.DisplayProgressBar("Searching", "Finding all prefabs in selection", progress);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
        #endregion

        #region Apply/Revert
        private void ApplySelectedPrefabs() {
            EditorUtility.DisplayProgressBar("Searching", "Finding all prefabs in selection", 0.0f);
            float progress = 0;

            foreach (Reference reference in references) {
                if (reference.Selected) {
                    EditorUtility.DisplayProgressBar("Searching", "Applying modifications to selected prefabs", progress++);

                    PrefabUtility.ReplacePrefab(reference.Prefab, PrefabUtility.GetPrefabParent(reference.Prefab), ReplacePrefabOptions.ConnectToPrefab);
                }
            }

            EditorUtility.ClearProgressBar();
        }
        
        private void RevertSelectedPrefabs() {
            EditorUtility.DisplayProgressBar("Searching", "Finding all prefabs in selection", 0.0f);
            float progress = 0;

            foreach (Reference reference in references) {
                if (reference.Selected) {
                    EditorUtility.DisplayProgressBar("Searching", "Reverting modifications to selected prefabs", progress++);

                    PrefabUtility.ReconnectToLastPrefab(reference.Prefab);
                    PrefabUtility.RevertPrefabInstance(reference.Prefab);
                }
            }

            EditorUtility.ClearProgressBar();
        }
        #endregion

        #region Utilities
        private void UpdateSelection(Reference aReference, bool aSelected) {
            aReference.Selected = aSelected;
        }
        #endregion

    }

}
