using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

  [CustomPropertyDrawer(typeof(LimitedVector3Attribute))]
  public class LimitedVector3AttributeDrawer : PropertyDrawer {
    #region GUI Functions
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      if (property.type == "LimitedVector3") {
        LimitedVector3Attribute range = attribute as LimitedVector3Attribute;
        SerializedProperty min = property.FindPropertyRelative("_min");
        SerializedProperty max = property.FindPropertyRelative("_max");
        Vector3 newMin = min.vector3Value;
        Vector3 newMax = max.vector3Value;

        // Property Label
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

        float yDivision = position.height / 4.0f;

        // X Value
        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(position.x + 20.0f, position.y + yDivision, 20.0f, EditorGUIUtility.singleLineHeight), "X");
        newMin.x = EditorGUI.FloatField(new Rect(position.x + 40.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMin.x);
        newMax.x = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + yDivision, 30.0f, EditorGUIUtility.singleLineHeight), newMax.x);

        EditorGUI.MinMaxSlider(new Rect(position.x + 80.0f, position.y + yDivision, position.width - 120.0f, EditorGUIUtility.singleLineHeight), ref newMin.x, ref newMax.x, range.Min.x, range.Max.x);

        // Y value
        EditorGUI.LabelField(new Rect(position.x + 20.0f, position.y + (yDivision * 2), 20.0f, EditorGUIUtility.singleLineHeight), "Y");
        newMin.y = EditorGUI.FloatField(new Rect(position.x + 40.0f, position.y + (yDivision * 2), 30.0f, EditorGUIUtility.singleLineHeight), newMin.y);
        newMax.y = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + (yDivision * 2), 30.0f, EditorGUIUtility.singleLineHeight), newMax.y);

        EditorGUI.MinMaxSlider(new Rect(position.x + 80.0f, position.y + (yDivision * 2), position.width - 120.0f, EditorGUIUtility.singleLineHeight), ref newMin.y, ref newMax.y, range.Min.y, range.Max.y);

        // Z value
        EditorGUI.LabelField(new Rect(position.x + 20.0f, position.y + (yDivision * 3), 20.0f, EditorGUIUtility.singleLineHeight), "Z");
        newMin.z = EditorGUI.FloatField(new Rect(position.x + 40.0f, position.y + (yDivision * 3), 30.0f, EditorGUIUtility.singleLineHeight), newMin.z);
        newMax.z = EditorGUI.FloatField(new Rect(position.x + position.width - 30.0f, position.y + (yDivision * 3), 30.0f, EditorGUIUtility.singleLineHeight), newMax.z);

        EditorGUI.MinMaxSlider(new Rect(position.x + 80.0f, position.y + (yDivision * 3), position.width - 120.0f, EditorGUIUtility.singleLineHeight), ref newMin.z, ref newMax.z, range.Min.z, range.Max.z);

        // Update values
        if (EditorGUI.EndChangeCheck()) {
          min.vector3Value = newMin;
          max.vector3Value = newMax;
        }
      } else {
        Debug.LogError(string.Format("Invalid type [{0}] for LimitedVector3Attribute", property.type));
      }
    }
    #endregion

    #region Utilities
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      int lineCount = 4;
      return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
    }
    #endregion

  }

}
