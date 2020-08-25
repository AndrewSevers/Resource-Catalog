using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

  [CustomPropertyDrawer(typeof(LimitedVector2Attribute))]
  public class LimitedVector2AttributeDrawer : PropertyDrawer {
    #region GUI Functions
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      if (property.type == "LimitedVector2") {
        LimitedVector2Attribute range = attribute as LimitedVector2Attribute;
        SerializedProperty min = property.FindPropertyRelative("_min");
        SerializedProperty max = property.FindPropertyRelative("_max");
        Vector2 newMin = min.vector2Value;
        Vector2 newMax = max.vector2Value;

        // Property Label
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

        float yDivision = position.height / 3.0f;

        // X Value
        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(position.x + 20.0f, position.y + yDivision, 20.0f, EditorGUIUtility.singleLineHeight), "X");
        newMin.x = EditorGUI.FloatField(new Rect(position.x + 40.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMin.x);
        newMax.x = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMax.x);

        EditorGUI.MinMaxSlider(new Rect(position.x + 80.0f, position.y + yDivision, position.width - 120.0f, yDivision), ref newMin.x, ref newMax.x, range.Min.x, range.Max.x);

        // Y value
        EditorGUI.LabelField(new Rect(position.x + 20.0f, position.y + (yDivision * 2), 20.0f, EditorGUIUtility.singleLineHeight), "Y");
        newMin.y = EditorGUI.FloatField(new Rect(position.x + 40.0f, position.y + (yDivision * 2), 30.0f, EditorGUIUtility.singleLineHeight), newMin.y);
        newMax.y = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + (yDivision * 2), 30.0f, EditorGUIUtility.singleLineHeight), newMax.y);

        EditorGUI.MinMaxSlider(new Rect(position.x + 80.0f, position.y + (yDivision * 2), position.width - 120.0f, EditorGUIUtility.singleLineHeight), ref newMin.y, ref newMax.y, range.Min.y, range.Max.y);

        // Update values
        if (EditorGUI.EndChangeCheck()) {
          min.vector2Value = newMin;
          max.vector2Value = newMax;
        }
      } else {
        Debug.LogError(string.Format("Invalid type [{0}] for LimitedVector2Attribute", property.type));
      }
    }
    #endregion

    #region Utilities
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      int lineCount = 3;
      return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
    }
    #endregion

  }

}
