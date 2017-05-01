using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer {

        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            aProperty.intValue = EditorGUI.MaskField(aRect, aLabel, aProperty.intValue, aProperty.enumDisplayNames);
        }

    }
}
