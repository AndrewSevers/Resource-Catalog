using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    public class LimitedRangeAttribute : PropertyAttribute {
        private float minLimit, maxLimit;

        #region Getters & Setters
        public float MinLimit {
            get { return minLimit; }
            set { minLimit = value; }
        }

        public float MaxLimit {
            get { return maxLimit; }
            set { maxLimit = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
        /// If the given property is set to false (inactive) then this property will be disabled.
        /// </summary>
        /// <param name="aMinLimit">Minimum value the range can be</param>
        /// <param name="aMaxLimit">Maximum value the range can be</param>
        public LimitedRangeAttribute(float aMinLimit = 0, float aMaxLimit = 100) {
            minLimit = aMinLimit;
            maxLimit = aMaxLimit;
        }
        #endregion

    }

    #region Limited Range Class
    [System.Serializable]
    public struct LimitedRange {
        [SerializeField]
        private float min, max;

        #region Statics
        private static LimitedRange ten = new LimitedRange(0.0f, 10.0f);
        private static LimitedRange hundred = new LimitedRange(0.0f, 100.0f);
        #endregion

        #region Getters & Setters
        public float Min {
            get { return min; }
            set { min = value; }
        }

        public int MinToInt {
            get { return (int) min; }
        }

        public float Max {
            get { return max; }
            set { max = value; }
        }

        public int MaxToInt {
            get { return (int) max; }
        }

        public static LimitedRange ToTen {
            get { return ten; }
        }

        public static LimitedRange ToHundred {
            get { return hundred; }
        }
        #endregion

        #region Constructors
        public LimitedRange(float aMin, float aMax) {
            min = aMin;
            max = aMax;
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Check to see if the given value is within the clamped range
        /// </summary>
        /// <param name="aValue"></param>
        public bool WithinRange(float aValue) {
            return (aValue <= max && aValue >= min);
        }

        /// <summary>
        /// Check to see if the given values are within the clamped range. If they are not the referenced index will contain the first invalid index.
        /// </summary>
        /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
        /// <param name="aValues">Values to compare</param>
        public bool WithinRange(ref int aIndex, params float[] aValues) {
            aIndex = -1;

            for (int i = 0; i < aValues.Length; i++) {
                float value = aValues[i];
                if (value > max || value < min) {
                    aIndex = i;
                    break;
                }
            }

            return (aIndex == -1);
        }

        /// <summary>
        /// Get a random value within the min/max range
        /// </summary>
        public float Random() {
            if (min != max) {
                return UnityEngine.Random.Range(min, max);
            } else {
                return min;
            }
        }

        /// <summary>
        /// Get a random int value within the min/max range
        /// </summary>
        public int RandomToInt() {
            if (min != max) {
                return Mathf.RoundToInt(UnityEngine.Random.Range(min, max));
            } else {
                return Mathf.RoundToInt(min);
            }
        }
        #endregion

    }
    #endregion

}
