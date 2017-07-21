using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Only allow the value to be within the contrained bounds and display in the inspector as a multi-bounded slider for all vector values
    /// </summary>
    public class LimitedVector2Attribute : PropertyAttribute {
        private Vector2 minLimit, maxLimit;

        #region Getters & Setters
        public Vector2 MinLimit {
            get { return minLimit; }
            set { minLimit = value; }
        }

        public Vector2 MaxLimit {
            get { return maxLimit; }
            set { maxLimit = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
        /// If the given property is set to false (inactive) then this property will be disabled.
        /// </summary>
        /// <param name="aMinX">Minimum x value the vector can be</param>
        /// <param name="aMaxX">Minimum y value the vector can be</param>
        /// <param name="aMaxX">Maximum x value the vector can be</param>
        /// <param name="aMaxY">Maximum y value the vector can be</param>
        public LimitedVector2Attribute(float aMinX, float aMaxX, float aMinY, float aMaxY) {
            minLimit = new Vector2(aMinX, aMinY);
            maxLimit = new Vector2(aMaxX, aMaxY);
        }

        /// <summary>
        /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
        /// If the given property is set to false (inactive) then this property will be disabled.
        /// </summary>
        /// <param name="aMinLimit">Minimum value the vector can be</param>
        /// <param name="aMaxLimit">Maximum value the vector can be</param>
        public LimitedVector2Attribute(Vector2 aMinLimit, Vector2 aMaxLimit) {
            minLimit = aMinLimit;
            maxLimit = aMaxLimit;
        }
        #endregion

    }

    #region Clamped Range Class
    [System.Serializable]
    public struct LimitedVector2 {
        [SerializeField]
        private Vector2 min, max;

        #region Statics
        private static LimitedVector2 ten = new LimitedVector2(Vector2.zero, new Vector2(10.0f, 10.0f));
        private static LimitedVector2 hundred = new LimitedVector2(Vector2.zero, new Vector2(100.0f, 100.0f));
        #endregion

        #region Getters & Setters
        public Vector2 Min {
            get { return min; }
            set { min = value; }
        }

        public Vector2 Max {
            get { return max; }
            set { max = value; }
        }

        public static LimitedVector2 ToTen {
            get { return ten; }
        }

        public static LimitedVector2 ToHundred {
            get { return hundred; }
        }
        #endregion

        #region Constructors
        public LimitedVector2(float aMinX, float aMinY, float aMaxX, float aMaxY) {
            min = new Vector2(aMinX, aMinY);
            max = new Vector2(aMaxX, aMaxY);
        }

        public LimitedVector2(Vector2 aMin, Vector2 aMax) {
            min = aMin;
            max = aMax;
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Check to see if the given value is within the limited range
        /// </summary>
        /// <param name="aValue"></param>
        public bool WithinRange(Vector2 aValue) {
            return (aValue.x <= max.x && aValue.x >= min.x && aValue.y <= max.y && aValue.y >= min.y);
        }

        /// <summary>
        /// Check to see if the given values are within the limited range. If they are not the referenced index will contain the first invalid index.
        /// </summary>
        /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
        /// <param name="aValues">Values to compare</param>
        public bool WithinRange(ref int aIndex, params Vector2[] aValues) {
            aIndex = -1;

            for (int i = 0; i < aValues.Length; i++) {
                Vector2 value = aValues[i];
                if (value.x > max.x || value.x < min.x || value.y > max.y || value.y < min.y) {
                    aIndex = i;
                    break;
                }
            }

            return (aIndex == -1);
        }

        /// <summary>
        /// Get a random value within the min/max range
        /// </summary>
        public Vector2 Random() {
            if (min != max) {
                return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
            } else {
                return min;
            }
        }
        #endregion

    }
    #endregion

}
