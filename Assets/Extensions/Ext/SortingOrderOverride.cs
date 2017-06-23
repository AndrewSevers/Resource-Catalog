using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions {

    [RequireComponent(typeof(Renderer))]
    public class SortingOrderOverride : MonoBehaviour {
        [SerializeField]
        private string sortingLayerName;
        [SerializeField]
        private int sortingOrder;


        #region Initialization
        private void Start() {
            UpdateRenderer();
        }
        #endregion

        #region Renderer Management
        [ContextMenu("Update Renderer")]
        public void UpdateRenderer() {
            Renderer renderer = GetComponent<Renderer>();

            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = sortingOrder;
        }
        #endregion

    }

}
