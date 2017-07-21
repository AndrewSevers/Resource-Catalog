using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(VisibilityAttribute))]
    public class VisibilityAttributeDrawer : PropertyDrawer {
        private SerializedProperty conditionalProperty;

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            VisibilityAttribute visibilityAttribute = attribute as VisibilityAttribute;

            if (conditionalProperty == null) {
                conditionalProperty = aProperty.serializedObject.FindProperty(visibilityAttribute.PropertyName);

                if (conditionalProperty == null) {
                    EditorGUI.LabelField(aRect, aLabel.text, "RequiredProperty must require a bool property.");
                    return;
                }
            }

            bool enableGUI = GUI.enabled;

            switch (conditionalProperty.propertyType) {
                case SerializedPropertyType.Boolean:
                    // Enable/Disable the GUI if the condition is matched
                    bool conditionMatched = (visibilityAttribute.Result == conditionalProperty.boolValue);
                    GUI.enabled = conditionMatched;

                    // If the visibily is hidden then don't show. If the visibility is disabled then show with a disabled GUI.
                    if (conditionMatched || visibilityAttribute.HideInInspector == false) {
                        EditorGUI.PropertyField(aRect, aProperty, true);
                    }

                    GUI.enabled = enableGUI;
                    break;
                case SerializedPropertyType.String:
                    // Enable/Disable the GUI if the condition is matched
                    conditionMatched = (visibilityAttribute.Result == (string.IsNullOrEmpty(conditionalProperty.stringValue) == false));
                    GUI.enabled = conditionMatched;

                    // If the visibily is hidden then don't show. If the visibility is disabled then show with a disabled GUI.
                    if (conditionMatched || visibilityAttribute.HideInInspector == false) {
                        EditorGUI.PropertyField(aRect, aProperty, true);
                    }

                    GUI.enabled = enableGUI;
                    break;
                case SerializedPropertyType.Enum:
                    // Enable/Disable the GUI if the condition is matched
                    conditionMatched = (visibilityAttribute.GetExpectedValue<int>() == conditionalProperty.enumValueIndex);
                    GUI.enabled = conditionMatched;

                    // If the visibily is hidden then don't show. If the visibility is disabled then show with a disabled GUI.
                    if (conditionMatched || visibilityAttribute.HideInInspector == false) {
                        EditorGUI.PropertyField(aRect, aProperty, true);
                    }

                    GUI.enabled = enableGUI;
                    break;
                default:
                    // Enable/Disable the GUI if the condition is matched
                    conditionMatched = (visibilityAttribute.Result == (conditionalProperty.objectReferenceValue != null));
                    GUI.enabled = conditionMatched;

                    // If the visibily is hidden then don't show. If the visibility is disabled then show with a disabled GUI.
                    if (conditionMatched || visibilityAttribute.HideInInspector == false) {
                        EditorGUI.PropertyField(aRect, aProperty, true);
                    }

                    GUI.enabled = enableGUI;
                    break;
            }
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            VisibilityAttribute visibilityAttribute = attribute as VisibilityAttribute;

            if (conditionalProperty == null) {
                conditionalProperty = aProperty.serializedObject.FindProperty(visibilityAttribute.PropertyName);
                if (conditionalProperty == null) {
                    throw new System.Exception("[VisibilityAttributeDrawer] Wanted property \"" + visibilityAttribute.PropertyName + "\" not found");
                }
            }

            switch (conditionalProperty.propertyType) {
                case SerializedPropertyType.Boolean:
                    return (conditionalProperty.boolValue == visibilityAttribute.Result || visibilityAttribute.HideInInspector == false) ? EditorGUI.GetPropertyHeight(aProperty) : -EditorGUIUtility.standardVerticalSpacing;
                case SerializedPropertyType.String:
                    return ((string.IsNullOrEmpty(conditionalProperty.stringValue) == false) == visibilityAttribute.Result || visibilityAttribute.HideInInspector == false) ? EditorGUI.GetPropertyHeight(aProperty) : -EditorGUIUtility.standardVerticalSpacing;
                case SerializedPropertyType.Enum:
                    return (visibilityAttribute.GetExpectedValue<int>() == conditionalProperty.enumValueIndex || visibilityAttribute.HideInInspector == false) ? EditorGUI.GetPropertyHeight(aProperty) : -EditorGUIUtility.standardVerticalSpacing;
                default:
                    return ((conditionalProperty.objectReferenceValue != null) == visibilityAttribute.Result || visibilityAttribute.HideInInspector == false) ? EditorGUI.GetPropertyHeight(aProperty) : -EditorGUIUtility.standardVerticalSpacing;
            }
        }
        #endregion

    }

}
