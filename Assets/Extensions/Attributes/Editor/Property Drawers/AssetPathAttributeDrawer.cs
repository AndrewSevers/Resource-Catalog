using UnityEditor;
using UnityEngine;

namespace Extensions.Properties {

  [CustomPropertyDrawer(typeof(AssetPathAttribute))]
  public class AssetPathAttributeDrawer : PropertyDrawer {
    private bool showPath = false;
    private bool isAsset = false;
    private Object asset = null;
    AssetPathAttribute assetAttribute;

    private Texture textIcon = Resources.Load<Texture>("Images/ab-bold");
    private Texture objectIcon = Resources.Load<Texture>("Images/box-3d");

    #region GUI Functions
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      string associatedPath;

      assetAttribute = attribute as AssetPathAttribute;

      switch (property.propertyType) {
        case SerializedPropertyType.String:
          associatedPath = (assetAttribute.AssetType == AssetType.Asset) ? property.stringValue : FormatToPath(property.stringValue, AssetType.Resource, AssetType.Asset);
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
          if (property.type == "Asset") {
            associatedPath = property.FindPropertyRelative("assetPath").stringValue;
            isAsset = true;

            break;
          } else {
            EditorGUI.LabelField(rect, label.text, "Property must be a 'string' or 'Asset' property.");
          }

          return;
      }

      asset = AssetDatabase.LoadAssetAtPath(associatedPath, typeof(Object));

      DisplayPathGUI(rect, property, label);
    }

    private void DisplayPathGUI(Rect rect, SerializedProperty property, GUIContent label) {
      string path = null;

      Rect original = new Rect(rect);
      rect.width -= 30.0f;


      if (showPath) {
        if (isAsset) {
          SerializedProperty assetPath = property.FindPropertyRelative("assetPath");
          SerializedProperty resourcePath = property.FindPropertyRelative("resourcePath");

          EditorGUI.BeginChangeCheck();

          path = EditorGUI.TextField(rect, label, assetPath.stringValue);

          if (EditorGUI.EndChangeCheck()) {
            assetPath.stringValue = path;
            resourcePath.stringValue = FormatToPath(path, AssetType.Asset, AssetType.Resource);
          }
        } else {
          EditorGUI.PropertyField(rect, property);
        }
      } else {
        EditorGUI.BeginChangeCheck();

        asset = EditorGUI.ObjectField(rect, label, asset, typeof(Object), false);

        if (asset != null) {
          path = AssetDatabase.GetAssetPath(asset);
        }

        if (EditorGUI.EndChangeCheck()) {
          if (isAsset) {
            SerializedProperty assetPath = property.FindPropertyRelative("assetPath");
            SerializedProperty resourcePath = property.FindPropertyRelative("resourcePath");

            assetPath.stringValue = path;
            resourcePath.stringValue = FormatToPath(path, AssetType.Asset, AssetType.Resource);
          } else {
            property.stringValue = FormatPath(path);
          }
        }
      }

      original.x = original.width - (25.0f / 2);
      original.width = 30.0f;

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
    /// <param name="path">Path for format</param>
    private string FormatPath(string path) {
      AssetType type = (attribute as AssetPathAttribute).AssetType;

      switch (type) {
        case AssetType.Resource:
          return FormatToPath(path, AssetType.Asset, AssetType.Resource);
        default:
          return path;
      }
    }

    /// <summary>
    /// Format from path format to new path format
    /// </summary>
    /// <param name="path">Path to format</param>
    /// <param name="fromType">Starting type</param>
    /// <param name="toType">Final type</param>
    private string FormatToPath(string path, AssetType fromType = AssetType.Asset, AssetType toType = AssetType.Resource) {
      switch (fromType) {
        case AssetType.Resource: // Resource format -> Any
          switch (toType) {
            case AssetType.Asset:
              Debug.LogWarning(string.Format("Asset: {0}", path));
              return "Assets/" + path;
          }

          break;
        case AssetType.Asset:  // Asset format -> any
          switch (toType) {
            case AssetType.Resource:
              path = path.Remove(path.LastIndexOf('.'));
              path = path.Substring(path.LastIndexOf("Resources/") + "Resources/".Length);

              Debug.LogWarning(string.Format("Asset: {0}", path));
              return path;
          }

          break;
      }

      return null;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      if (property.type == "Asset") {
        return EditorGUI.GetPropertyHeight(property, label) / 3.0f;
      } else {
        return EditorGUI.GetPropertyHeight(property, label);
      }
    }
    #endregion

  }

}
