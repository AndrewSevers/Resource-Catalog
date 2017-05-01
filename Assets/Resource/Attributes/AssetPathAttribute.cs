using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Display the given field as a string with optional dialog finder window
    /// </summary>
    public class AssetPathAttribute : PropertyAttribute {
        private AssetType type;
        private SystemType systemType;

        #region Getters & Setters
        public AssetType AssetType {
            get { return type; }
        }

        public SystemType SystemType {
            get { return systemType; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Configure an interface that supports object drag and drop to determine the the file's path
        /// </summary>
        /// <param name="aType">Type of the asset to load. The produced string depends on the type.</param>
        public AssetPathAttribute(AssetType aType = AssetType.Resource, SystemType aSystemType = SystemType.GameObject) {
            type = aType;
            systemType = aSystemType;
        }
        #endregion
    }

    [System.Serializable]
    public class Asset {
        [SerializeField]
        private string resourcePath;
        [SerializeField]
        private string assetPath;

        #region Getters & Setters
        public string ResourcePath {
            get { return resourcePath; }
        }

        public string AssetPath {
            get { return assetPath; }
        }
        #endregion

        #region Constructor
        public Asset(string aResourcePath = null, string aAssetPath = null) {
            resourcePath = aResourcePath;
            assetPath = aAssetPath;
        }
        #endregion
    }

    public enum AssetType {
        Asset,
        Resource
    }

    public enum SystemType {
        GameObject,
        AudioClip,
        Text,
        Material,
        Shader,
        Json
    }

}
