using System;
using UnityEngine;

namespace Extensions.Properties {

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
        /// Display the attached property based on whether or not the given, or selected, property's state matches the given result
        /// </summary>
        /// <param name="aPropertyName">Property to compare</param>
        /// <param name="aExpectedResult">Result to expect</param>
        /// <param name="aHideInInspector">Whether or not the attached property is hidden completely from the inspector if the result doesn't match</param>
        public VisibilityAttribute(string aPropertyName, bool aExpectedResult, bool aHideInInspector = true) {
            propertyName = aPropertyName;
            result = aExpectedResult;
            hideInInspector = aHideInInspector;
        }

        /// <summary>
        /// Display the attached property based on whether or not the given, or selected, property's value matches the given value
        /// </summary>
        /// <param name="aPropertyName">Property to compare</param>
        /// <param name="aExpectedValue">Possible value to compare against</param>
        /// <param name="aHideInInspector">Whether or not the attached property is hidden completely from the inspector if the result doesn't match</param>
		public VisibilityAttribute(string aPropertyName, object aExpectedValue, bool aHideInInspector = true) {
			propertyName = aPropertyName;
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
