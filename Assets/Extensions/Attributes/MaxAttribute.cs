using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Force the int/float value to never exceed the maximum provided value.
    /// </summary>
    public class MaxAttribute : PropertyAttribute {
        private float maxValue;

        #region Getters & Setters
        public float MaxValue {
            get { return maxValue; }
        }
        #endregion

        #region Constructor
        public MaxAttribute(int aMaximumValue) {
            maxValue = aMaximumValue;
        }

        public MaxAttribute(float aMaximumValue) {
            maxValue = aMaximumValue;
        }
        #endregion

    }

}
