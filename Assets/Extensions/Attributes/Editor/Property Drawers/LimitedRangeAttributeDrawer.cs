using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

  [CustomPropertyDrawer(typeof(LimitedRangeAttribute))]
  public class LimitedRangeAttributeDrawer : PropertyDrawer {
    #region GUI Functions
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      if (property.type == "LimitedRange") {
        LimitedRangeAttribute range = attribute as LimitedRangeAttribute;
        SerializedProperty min = property.FindPropertyRelative("_min");
        SerializedProperty max = property.FindPropertyRelative("_max");

        float newMin = min.floatValue;
        float newMax = max.floatValue;

        // Property Label
        EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

        float yDivision = position.height / 2.0f;

        EditorGUI.BeginChangeCheck();
        newMin = EditorGUI.FloatField(new Rect(position.x + 20.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMin);
        newMax = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMax);

        EditorGUI.MinMaxSlider(new Rect(position.x + 60.0f, position.y + yDivision, position.width - 100.0f, EditorGUIUtility.singleLineHeight), ref newMin, ref newMax, range.Min, range.Max);

        // Update values
        if (EditorGUI.EndChangeCheck()) {
          min.floatValue = Mathf.Clamp(newMin, range.Min, range.Max);
          max.floatValue = Mathf.Clamp(newMax, range.Min, range.Max);
        }
      } else {
        Debug.LogError(string.Format("Invalid type [{0}] for LimitedRangeAttribute", property.type));
      }
    }
    #endregion

    #region Utilities
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      int lineCount = 2;
      return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
    }
    #endregion

  }

}
