using UnityEngine;
using UnityEditor;

namespace Extensions.Properties {

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(aRect, aProperty, true);
            EditorGUI.EndDisabledGroup();
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty, aLabel);
        }
        #endregion

    }

}
