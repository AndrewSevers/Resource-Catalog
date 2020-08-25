using UnityEngine;
using UnityEditor;

namespace Extensions.Properties {

  [CustomPropertyDrawer(typeof(MaxAttribute))]
  public class MaxAttributeDrawer : PropertyDrawer {

    #region GUI Functions
    public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
      MaxAttribute maxAttribute = attribute as MaxAttribute;

      switch (aProperty.propertyType) {
        case SerializedPropertyType.Integer:
          aProperty.intValue = (int) Mathf.Clamp(aProperty.intValue, int.MinValue, maxAttribute.MaxValue);
          EditorGUI.PropertyField(aRect, aProperty, true);
          break;
        case SerializedPropertyType.Float:
          aProperty.floatValue = Mathf.Clamp(aProperty.floatValue, float.MinValue, maxAttribute.MaxValue);
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
