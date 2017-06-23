using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Provide a set of contraints (along various axes)
    /// </summary>
    public class AxesConstraintAttribute : PropertyAttribute {

    }

    [System.Serializable]
    public class AxesConstraint {
        [SerializeField]
        private bool x;
        [SerializeField]
        private bool y;
        [SerializeField]
        private bool z;

        #region Getters & Setters
        public bool X {
            get { return x; }
            set { x = value; }
        }

        public bool Y {
            get { return y; }
            set { y = value; }
        }

        public bool Z {
            get { return z; }
            set { z = value; }
        }
        #endregion

        #region Constructor
        public AxesConstraint(bool aX, bool aY, bool aZ) {
            x = aX;
            y = aY;
            z = aZ;
        }
        #endregion

    }

}
