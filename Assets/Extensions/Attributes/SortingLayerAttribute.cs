using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Display the given field as a sorting layer property. 
    /// </summary>
    public class SortingLayerAttribute : PropertyAttribute {

    }

    [System.Serializable]
    public class SortingData {
        [SerializeField, SortingLayer]
        private string layer = "Default";
        [SerializeField]
        private int order = 0;

        #region Getters & Setters
        public string Layer {
            get { return layer; }
            set { layer = value; }
        }

        public int Order {
            get { return order; }
            set { order = value; }
        }
        #endregion

        #region Constructor
        public SortingData(string aLayer = "Default", int aOrder = 0) {
            layer = aLayer;
            order = aOrder;
        }
        #endregion
    }

}