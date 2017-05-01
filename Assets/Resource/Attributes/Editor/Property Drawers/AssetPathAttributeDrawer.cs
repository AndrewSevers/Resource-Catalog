using System.Text;
using UnityEditor;
using UnityEngine;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(AssetPathAttribute))]
    public class AssetPathAttributeDrawer : PropertyDrawer {
        private bool showPath = false;
        private bool isAsset = false;
        private Object asset = null;
        AssetPathAttribute assetAttribute;

        private Texture textIcon = Resources.Load<Texture>("Images/ab_bold");
        private Texture objectIcon = Resources.Load<Texture>("Images/box_3d");

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            string associatedPath;

            assetAttribute = attribute as AssetPathAttribute;

            switch (aProperty.propertyType) {
                case SerializedPropertyType.String:
                    associatedPath = (assetAttribute.AssetType == AssetType.Asset) ? aProperty.stringValue : FormatToPath(aProperty.stringValue, AssetType.Resource, AssetType.Asset);
                    switch (assetAttribute.SystemType) {
                        case SystemType.GameObject:
                            associatedPath += ".prefab";
                            break;
                        case SystemType.Material:
                            associatedPath += ".mat";
                            break;
                    }

                    isAsset = false;

                    break;
                default:
                    if (aProperty.type == "Asset") {
                        associatedPath = aProperty.FindPropertyRelative("assetPath").stringValue;
                        isAsset = true;

                        break;
                    } else {
                        EditorGUI.LabelField(aRect, aLabel.text, "Property must be a 'string' or 'Asset' property.");
                    }

                    return;
            }

            asset = AssetDatabase.LoadAssetAtPath(associatedPath, typeof(Object));

            DisplayPathGUI(aRect, aProperty, aLabel);
        }

        private void DisplayPathGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            string path = null;

            Rect original = new Rect(aRect);
            aRect.width -= 30.0f;

            if (showPath) {
                if (isAsset) {
                    SerializedProperty assetPath = aProperty.FindPropertyRelative("assetPath");
                    SerializedProperty resourcePath = aProperty.FindPropertyRelative("resourcePath");

                    EditorGUI.BeginChangeCheck();

                    path = EditorGUI.TextField(aRect, aLabel, assetPath.stringValue);

                    if (EditorGUI.EndChangeCheck()) {
                        assetPath.stringValue = path;
                        resourcePath.stringValue = FormatToPath(path, AssetType.Asset, AssetType.Resource);
                    }
                } else {
                    EditorGUI.PropertyField(aRect, aProperty);
                }
            } else {
                EditorGUI.BeginChangeCheck();

                asset = EditorGUI.ObjectField(aRect, aLabel, asset, typeof(Object), false);

                if (asset != null) {
                    path = AssetDatabase.GetAssetPath(asset);
                }

                if (EditorGUI.EndChangeCheck()) {
                    if (isAsset) {
                        SerializedProperty assetPath = aProperty.FindPropertyRelative("assetPath");
                        SerializedProperty resourcePath = aProperty.FindPropertyRelative("resourcePath");

                        assetPath.stringValue = path;
                        resourcePath.stringValue = FormatToPath(path, AssetType.Asset, AssetType.Resource);
                    } else {
                        aProperty.stringValue = FormatPath(path);
                    }
                }
            }

            original.x = original.width - (25.0f / 2);
            original.width = 25.0f;

            Texture icon = (showPath) ? objectIcon : textIcon;
            if (GUI.Button(original, icon)) {
                showPath = !showPath;
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Automatically format from Asset type to whatever the given type is defined in the attribute
        /// </summary>
        /// <param name="aPath">Path for format</param>
        private string FormatPath(string aPath) {
            AssetType type = (attribute as AssetPathAttribute).AssetType;

            switch (type) {
                case AssetType.Resource:
                    return FormatToPath(aPath, AssetType.Asset, AssetType.Resource);
                default:
                    return aPath;
            }
        }

        /// <summary>
        /// Format from path format to new path format
        /// </summary>
        /// <param name="aPath">Path to format</param>
        /// <param name="aFromType">Starting type</param>
        /// <param name="aToType">Final type</param>
        private string FormatToPath(string aPath, AssetType aFromType = AssetType.Asset, AssetType aToType = AssetType.Resource) {
            switch (aFromType) {
                case AssetType.Resource: // Resource format -> Any
                    switch (aToType) {
                        case AssetType.Asset:
                            return ("Assets/Resources/" + aPath);
                    }

                    break;
                case AssetType.Asset:  // Asset format -> any
                    switch (aToType) {
                        case AssetType.Resource:
                            aPath = aPath.Remove(aPath.LastIndexOf('.'));
                            aPath = aPath.Substring(aPath.LastIndexOf("Resources/") + "Resources/".Length);
                            return aPath;
                    }

                    break;
            }

            return null;
        }

        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "Asset") {
                return EditorGUI.GetPropertyHeight(aProperty, aLabel) / 3.0f;
            } else {
                return EditorGUI.GetPropertyHeight(aProperty, aLabel);
            }
        }
        #endregion

    }

}
