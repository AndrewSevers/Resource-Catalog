using UnityEditor;
using UnityEngine;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(LimitedVector2Attribute))]
    public class LimitedVector2AttributeDrawer : PropertyDrawer {
        private const float padding = 10.0f;

        #region GUI Functions
        public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "LimitedVector2") {
                LimitedVector2Attribute range = attribute as LimitedVector2Attribute;
                SerializedProperty min = aProperty.FindPropertyRelative("min");
                SerializedProperty max = aProperty.FindPropertyRelative("max");
                Vector2 newMin = min.vector2Value;
                Vector2 newMax = max.vector2Value;

                // Property Label
                EditorGUI.LabelField(new Rect(aPosition.x, aPosition.y, aPosition.width, aPosition.height), aLabel);

                float yDivision = aPosition.height / 3.0f;

                // X Value
                EditorGUI.BeginChangeCheck();
                EditorGUI.LabelField(new Rect(aPosition.x + 20.0f, aPosition.y + yDivision, 20.0f, yDivision), "X");
                newMin.x = EditorGUI.FloatField(new Rect(aPosition.x + 40.0f, aPosition.y + yDivision, 30.0f, yDivision), newMin.x);
                newMax.x = EditorGUI.FloatField(new Rect(aPosition.x + aPosition.width - 30.0f, aPosition.y + yDivision, 30.0f, yDivision), newMax.x);

                EditorGUI.MinMaxSlider(new Rect(aPosition.x + 80.0f, aPosition.y + yDivision, aPosition.width - 120.0f, yDivision), ref newMin.x, ref newMax.x, range.MinLimit.x, range.MaxLimit.x);

                // Y value
                EditorGUI.LabelField(new Rect(aPosition.x + 20.0f, aPosition.y + (yDivision * 2), 20.0f, yDivision), "Y");
                newMin.y = EditorGUI.FloatField(new Rect(aPosition.x + 40.0f, aPosition.y + (yDivision * 2), 30.0f, yDivision), newMin.y);
                newMax.y = EditorGUI.FloatField(new Rect(aPosition.x + aPosition.width - 30.0f, aPosition.y + (yDivision * 2), 30.0f, yDivision), newMax.y);

                EditorGUI.MinMaxSlider(new Rect(aPosition.x + 80.0f, aPosition.y + (yDivision * 2), aPosition.width - 120.0f, yDivision), ref newMin.y, ref newMax.y, range.MinLimit.y, range.MaxLimit.y);

                // Update values
                if (EditorGUI.EndChangeCheck()) {
                    min.vector2Value = newMin;
                    max.vector2Value = newMax;
                }
            } else {
                Debug.LogError(string.Format("Invalid type [{0}] for LimitedVector2Attribute", aProperty.type));
            }
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 3.0f;
        }
        #endregion

    }

}
