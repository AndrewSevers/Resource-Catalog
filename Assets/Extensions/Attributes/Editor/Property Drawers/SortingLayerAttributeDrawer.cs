using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerAttributeDrawer : PropertyDrawer {
        private string[] sortingLayerNames;
        private int sortingLayerIndex;
        private bool open = false;

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            switch (aProperty.propertyType) {
                case SerializedPropertyType.String:
                    if (sortingLayerNames == null) {
                        sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
                        sortingLayerIndex = GetLayerIndex(aProperty.stringValue);
                    }

                    EditorGUI.BeginChangeCheck();
                    int newIndex = EditorGUI.Popup(aRect, aLabel.text, sortingLayerIndex, sortingLayerNames);
                    if (EditorGUI.EndChangeCheck()) {
                        if (sortingLayerIndex != newIndex) {
                            sortingLayerIndex = newIndex;
                            aProperty.stringValue = SortingLayer.layers[sortingLayerIndex].name;
                        }
                    }

                    break;
                default:
                    if (aProperty.type == "SortingData") {
                        SerializedProperty layer = aProperty.FindPropertyRelative("layer");
                        SerializedProperty order = aProperty.FindPropertyRelative("order");

                        if (sortingLayerNames == null) {
                            sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
                            sortingLayerIndex = GetLayerIndex(layer.stringValue);
                        }

                        Rect foldoutRect = aRect;
                        foldoutRect.height = 16.5f;

                        open = EditorGUI.Foldout(foldoutRect, open, aLabel);
                        if (open) {
                            aRect.height /= 3;

                            aRect.y += 16.5f;
                            EditorGUI.BeginChangeCheck();
                            newIndex = EditorGUI.Popup(aRect, "Sorting Layer", sortingLayerIndex, sortingLayerNames);
                            if (EditorGUI.EndChangeCheck()) {
                                if (sortingLayerIndex != newIndex) {
                                    sortingLayerIndex = newIndex;

                                    layer.stringValue = SortingLayer.layers[sortingLayerIndex].name;
                                }
                            }

                            aRect.y += 16.5f;
                            order.intValue = EditorGUI.IntField(aRect, "Order in Layer", order.intValue);
                        }
                    } else {
                        EditorGUI.LabelField(aRect, aLabel.text, "Property must be a 'string' or 'SortingData' property.");
                    }

                    break;
            }
        }
        #endregion

        #region Utilities
        private int GetLayerIndex(string aLayerName) {
            return Mathf.Clamp(System.Array.IndexOf(sortingLayerNames, aLayerName), 0, int.MaxValue); ;
        }

        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "SortingData") {
                if (open) {
                    return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 3.0f;
                } else {
                    return EditorGUI.GetPropertyHeight(aProperty, aLabel);
                }
            } else {
                return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 2.0f;
            }
        }
        #endregion

    }

}
