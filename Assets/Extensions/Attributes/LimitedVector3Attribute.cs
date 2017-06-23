using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    public class LimitedVector3Attribute : PropertyAttribute {
        private Vector3 minLimit, maxLimit;

        #region Getters & Setters
        public Vector3 MinLimit {
            get { return minLimit; }
            set { minLimit = value; }
        }

        public Vector3 MaxLimit {
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
        /// <param name="aMaxX">Minimum x value the vector can be</param>
        /// <param name="aMinY">Maximum y value the vector can be</param>
        /// <param name="aMaxY">Maximum y value the vector can be</param>
        /// <param name="aMinZ">Maximum z value the vector can be</param>
        /// <param name="aMaxZ">Maximum z value the vector can be</param>
        public LimitedVector3Attribute(float aMinX = 0.0f, float aMaxX = 100.0f, float aMinY = 0.0f, float aMaxY = 100.0f, float aMinZ = 0.0f, float aMaxZ = 100.0f) {
            minLimit = new Vector3(aMinX, aMinY, aMinZ);
            maxLimit = new Vector3(aMaxX, aMaxY, aMaxZ);
        }

        /// <summary>
        /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
        /// If the given property is set to false (inactive) then this property will be disabled.
        /// </summary>
        /// <param name="aMinLimit">Minimum value the vector can be</param>
        /// <param name="aMaxLimit">Maximum value the vector can be</param>
        public LimitedVector3Attribute(Vector3 aMinLimit, Vector3 aMaxLimit) {
            minLimit = aMinLimit;
            maxLimit = aMaxLimit;
        }
        #endregion

    }

    #region Limited Range Class
    [System.Serializable]
    public struct LimitedVector3 {
        [SerializeField]
        private Vector3 min, max;

        #region Statics
        private static LimitedVector3 ten = new LimitedVector3(Vector3.zero, new Vector3(10.0f, 10.0f, 100.0f));
        private static LimitedVector3 hundred = new LimitedVector3(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));
        #endregion

        #region Getters & Setters
        public Vector3 Min {
            get { return min; }
            set { min = value; }
        }

        public Vector3 Max {
            get { return max; }
            set { max = value; }
        }

        public static LimitedVector3 ToTen {
            get { return ten; }
        }

        public static LimitedVector3 ToHundred {
            get { return hundred; }
        }
        #endregion

        #region Constructors
        public LimitedVector3(float aMinX, float aMaxX, float aMinY, float aMaxY, float aMinZ, float aMaxZ) {
            min = new Vector3(aMinX, aMinY, aMinZ);
            max = new Vector3(aMaxX, aMaxY, aMaxZ);
        }

        public LimitedVector3(Vector3 aMin, Vector3 aMax) {
            min = aMin;
            max = aMax;
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Check to see if the given value is within the limited range
        /// </summary>
        /// <param name="aValue"></param>
        public bool WithinRange(Vector3 aValue) {
            return (aValue.x <= max.x && aValue.x >= min.x && aValue.y <= max.y && aValue.y >= min.y && aValue.z <= max.z && aValue.z >= min.z);
        }

        /// <summary>
        /// Check to see if the given values are within the limited range. If they are not the referenced index will contain the first invalid index.
        /// </summary>
        /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
        /// <param name="aValues">Values to compare</param>
        public bool WithinRange(ref int aIndex, params Vector3[] aValues) {
            aIndex = -1;

            for (int i = 0; i < aValues.Length; i++) {
                Vector3 value = aValues[i];
                if (value.x > max.x || value.x < min.x || value.y > max.y || value.y < min.y || value.z > max.z || value.z < min.z) {
                    aIndex = i;
                    break;
                }
            }

            return (aIndex == -1);
        }

        /// <summary>
        /// Get a random value within the min/max range
        /// </summary>
        public Vector3 Random() {
            if (min != max) {
                return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
            } else {
                return min;
            }
        }
        #endregion

    }
    #endregion

}
