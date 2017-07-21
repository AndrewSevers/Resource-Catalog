using UnityEngine;
using UnityEditor;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(MinAttribute))]
    public class MinAttributeDrawer : PropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            MinAttribute minAttribute = attribute as MinAttribute;

            switch (aProperty.propertyType) {
                case SerializedPropertyType.Integer:
                    aProperty.intValue = (int) Mathf.Clamp(aProperty.intValue, minAttribute.MinValue, int.MaxValue);
                    EditorGUI.PropertyField(aRect, aProperty, true);
                    break;
                case SerializedPropertyType.Float:
                    aProperty.floatValue = Mathf.Clamp(aProperty.floatValue, minAttribute.MinValue, float.MaxValue);
                    EditorGUI.PropertyField(aRect, aProperty, true);
                    break;
                default:
                    EditorGUI.LabelField(aRect, aLabel.text, "Property must be either a int or float.");
                    break;
            }
            
            return;
        }
        #endregion

    }

}
