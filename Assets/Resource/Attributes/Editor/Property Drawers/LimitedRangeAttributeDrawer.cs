using UnityEditor;
using UnityEngine;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(LimitedRangeAttribute))]
    public class LimitedRangeAttributeDrawer : PropertyDrawer {
        #region GUI Functions
        public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "LimitedRange") {

                LimitedRangeAttribute range = attribute as LimitedRangeAttribute;
                SerializedProperty min = aProperty.FindPropertyRelative("min");
                SerializedProperty max = aProperty.FindPropertyRelative("max");
                float newMin = min.floatValue;
                float newMax = max.floatValue;

                // Property Label
                EditorGUI.LabelField(new Rect(aPosition.x, aPosition.y, aPosition.width, aPosition.height), aLabel);

                float yDivision = aPosition.height * 0.5f;

                EditorGUI.BeginChangeCheck();
                newMin = EditorGUI.FloatField(new Rect(aPosition.x + 20.0f, aPosition.y + yDivision, 30.0f, yDivision), newMin);
                newMax = EditorGUI.FloatField(new Rect(aPosition.x + aPosition.width - 30.0f, aPosition.y + yDivision, 30.0f, yDivision), newMax);

                EditorGUI.MinMaxSlider(new Rect(aPosition.x + 60.0f, aPosition.y + yDivision, aPosition.width - 100.0f, yDivision), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);

                // Update values
                if (EditorGUI.EndChangeCheck()) {
                    min.floatValue = Mathf.Clamp(newMin, range.MinLimit, range.MaxLimit);
                    max.floatValue = Mathf.Clamp(newMax, range.MinLimit, range.MaxLimit);
                }
            } else {
                Debug.LogError(string.Format("Invalid type [{0}] for LimitedRangeAttribute", aProperty.type));
            }
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 2.0f;
        }
        #endregion

    }

}
