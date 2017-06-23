using UnityEngine;

namespace Extensions {

    [System.Serializable]
    public class ActionEvent {
        [SerializeField, Tooltip("ID of the action")]
        private string id;
        [SerializeField, Tooltip("Delay before the action is executed")]
        private float delay;
        [SerializeField, Tooltip("Variant number if clone actions exist that each have unique properties")]
        private int variant;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }

        public float Delay {
            get { return delay; }
        }

        public int Variant {
            get { return variant; }
        }
        #endregion

    }

}
