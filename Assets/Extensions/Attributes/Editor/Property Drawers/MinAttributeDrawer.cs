using UnityEngine;
using UnityEditor;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(MinAttribute))]
    public class MinAttributeDrawer : PropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            MinAttribute minAttribute = attribute as MinAttribute;

            if (aProperty.type == "int") {
                aProperty.intValue = (int) Mathf.Clamp(aProperty.intValue, minAttribute.MinValue, int.MaxValue);
                EditorGUI.PropertyField(aRect, aProperty, true);
                return;
            }

            if (aProperty.type == "float") {
                aProperty.floatValue = Mathf.Clamp(aProperty.floatValue, minAttribute.MinValue, float.MaxValue);
                EditorGUI.PropertyField(aRect, aProperty, true);
                return;
            }

            EditorGUI.LabelField(aRect, aLabel.text, "Property must be either a int or float.");
            return;
        }
        #endregion

    }

}
