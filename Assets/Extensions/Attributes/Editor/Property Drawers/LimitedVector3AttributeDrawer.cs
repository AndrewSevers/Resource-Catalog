using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(LimitedVector3Attribute))]
    public class LimitedVector3AttributeDrawer : PropertyDrawer {
        private const float padding = 10.0f;

        #region GUI Functions
        public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "LimitedVector3") {
                LimitedVector3Attribute range = attribute as LimitedVector3Attribute;
                SerializedProperty min = aProperty.FindPropertyRelative("min");
                SerializedProperty max = aProperty.FindPropertyRelative("max");
                Vector3 newMin = min.vector3Value;
                Vector3 newMax = max.vector3Value;

                // Property Label
                EditorGUI.LabelField(new Rect(aPosition.x, aPosition.y, aPosition.width, aPosition.height), aLabel);

                float yDivision = aPosition.height / 4.0f;

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

                // Z value
                EditorGUI.LabelField(new Rect(aPosition.x + 20.0f, aPosition.y + (yDivision * 3), 20.0f, yDivision), "Z");
                newMin.z = EditorGUI.FloatField(new Rect(aPosition.x + 40.0f, aPosition.y + (yDivision * 3), 30.0f, yDivision), newMin.z);
                newMax.z = EditorGUI.FloatField(new Rect(aPosition.x + aPosition.width - 30.0f, aPosition.y + (yDivision * 3), 30.0f, yDivision), newMax.z);

                EditorGUI.MinMaxSlider(new Rect(aPosition.x + 80.0f, aPosition.y + (yDivision * 3), aPosition.width - 120.0f, yDivision), ref newMin.z, ref newMax.z, range.MinLimit.z, range.MaxLimit.z);

                // Update values
                if (EditorGUI.EndChangeCheck()) {
                    min.vector3Value = newMin;
                    max.vector3Value = newMax;
                }
            } else {
                Debug.LogError(string.Format("Invalid type [{0}] for LimitedVector3Attribute", aProperty.type));
            }
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 4.0f;
        }
        #endregion

    }

}
