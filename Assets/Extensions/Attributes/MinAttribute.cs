using UnityEngine;

namespace Extensions.Properties {

    /// <summary>
    /// Force the int/float value to never exceed the minimum provided value.
    /// </summary>
    public class MinAttribute : PropertyAttribute {
		private float minValue;

		#region Getters & Setters
		public float MinValue {
			get { return minValue; }
		}
		#endregion

		#region Constructor
		public MinAttribute(int aMinimumValue) {
			minValue = aMinimumValue;
		}

		public MinAttribute(float aMinimumValue) {
			minValue = aMinimumValue;
		}
		#endregion

	}

}
