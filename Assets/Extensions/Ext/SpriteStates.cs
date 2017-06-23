using UnityEngine;

namespace Extensions {

    [System.Serializable]
    public class SpriteStates {
        [SerializeField]
        private Sprite normal;
        [SerializeField]
        private Sprite pressed;
        [SerializeField]
        private Sprite highlighted;
        [SerializeField]
        private Sprite disabled;

        #region Getters & Setters
        public Sprite Normal {
            get { return normal; }
        }

        public Sprite Pressed {
            get { return pressed; }
        }

        public Sprite Highlighted {
            get { return highlighted; }
        }

        public Sprite Disabled {
            get { return disabled; }
        }
        #endregion

    }

}
