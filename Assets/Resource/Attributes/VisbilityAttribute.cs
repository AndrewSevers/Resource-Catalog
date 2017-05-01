using System;
using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Display property based on whether the selected property matches the expected result (with an optional expected value as well)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class VisibilityAttribute : PropertyAttribute {
		private string propertyName;
        private object expectedValue;
        private bool result;
		private bool hideInInspector;

		#region Getters & Setters
		public string PropertyName {
			get { return propertyName; }
		}

        public bool Result {
            get { return result; }
        }

		public bool HideInInspector {
			get { return hideInInspector; }
		}
		#endregion

		#region Constructor
        /// <summary>
        /// Display the attached property based on whether or not the given, or selected, property matches the given result
        /// </summary>
        /// <param name="aPropertyName">Property to compare</param>
        /// <param name="aResult">Result to expect</param>
        /// <param name="aExpectedValue">Possible value to compare against</param>
        /// <param name="aHideInInspector">Whether or not the attached property is hidden completely from the inspector if the result doesn't match</param>
		public VisibilityAttribute(string aPropertyName, bool aResult, object aExpectedValue = null, bool aHideInInspector = true) {
			propertyName = aPropertyName;
			result = aResult;
            expectedValue = aExpectedValue;
			hideInInspector = aHideInInspector;
        }
        #endregion

        #region Utilities
        public T GetExpectedValue<T>() {
            return (T) expectedValue;
        }
        #endregion

    }

}
