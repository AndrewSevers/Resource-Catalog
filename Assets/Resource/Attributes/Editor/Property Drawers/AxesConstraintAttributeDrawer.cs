using UnityEditor;
using UnityEngine;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(AxesConstraintAttribute))]
    public class AxesConstraintAttributeDrawer : PropertyDrawer {
        private bool shown;

        #region GUI Functions
        public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "AxesConstraint") {
                SerializedProperty x = aProperty.FindPropertyRelative("x");
                SerializedProperty y = aProperty.FindPropertyRelative("y");
                SerializedProperty z = aProperty.FindPropertyRelative("z");
                bool newX = x.boolValue;
                bool newY = y.boolValue;
                bool newZ = z.boolValue;

                // Property Label
                shown = EditorGUI.Foldout(new Rect(aPosition.x, aPosition.y, aPosition.width, aPosition.height / 1.5f), shown, aLabel);

                if (shown) {
                    float yDivision = aPosition.height * 0.6f;

                    EditorGUI.BeginChangeCheck();

                    newX = EditorGUI.Toggle(new Rect(aPosition.x + 20.0f, aPosition.y + yDivision, 15, yDivision), newX);
                    EditorGUI.LabelField(new Rect(aPosition.x + 35.0f, aPosition.y + yDivision, 20, yDivision), "X");

                    newY = EditorGUI.Toggle(new Rect(aPosition.x + 70.0f, aPosition.y + yDivision, 15, yDivision), newY);
                    EditorGUI.LabelField(new Rect(aPosition.x + 85.0f, aPosition.y + yDivision, 20, yDivision), "Y");

                    newZ = EditorGUI.Toggle(new Rect(aPosition.x + 120.0f, aPosition.y + yDivision, 15, yDivision), newZ);
                    EditorGUI.LabelField(new Rect(aPosition.x + 135.0f, aPosition.y + yDivision, 20, yDivision), "Z");

                    // Update values
                    if (EditorGUI.EndChangeCheck()) {
                        x.boolValue = newX;
                        y.boolValue = newY;
                        z.boolValue = newZ;
                    }
                }
            } else {
                Debug.LogError(string.Format("Invalid type [{0}] for AxesConstraint", aProperty.type));
            }
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return (shown) ? EditorGUI.GetPropertyHeight(aProperty, aLabel) * 2.0f : EditorGUI.GetPropertyHeight(aProperty, aLabel);
        }
        #endregion

    }

}
