using UnityEngine;

namespace Extensions.Properties {

	/// <summary>
	/// Display the given field as a read-only variable. 
	/// Disallows any modifications to take place in the inspector.
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
