#if ANALYATICS
using Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions {

    public class AnalyticsManager : Singleton<AnalyticsManager> {
        [SerializeField]
        private string iosKey;
        [SerializeField]
        private string androidKey;

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            Flurry.Instance.StartSession(iosKey, androidKey);
        }
        #endregion

        #region Logging
        /// <summary>
        /// Log an event with a specific ID
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        public void LogEvent(string aEventID) {
            LogEvent(aEventID, null);
        }

        /// <summary>
        /// Log an event with a specific ID and a single parameter set
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameterID">ID of the parameter set</param>
        /// <param name="aParameterValue">Value of the parameter set</param>
        public void LogEvent(string aEventID, string aParameterID, string aParameterValue) {
            LogEvent(aEventID, new Dictionary<string, string>() { { aParameterID, aParameterValue } });
        }

        /// <summary>
        /// Log an event with a specfic ID with a dictionary of parameters
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameters">Dictionary containing all the parameters</param>
        public void LogEvent(string aEventID, Dictionary<string, string> aParameters) {
            if (aParameters != null) {
                Flurry.Instance.LogEvent(aEventID, aParameters);
            } else {
                Flurry.Instance.LogEvent(aEventID);
            }
        }

        /// <summary>
        /// Log the start of an event with a specfic ID
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        public void LogEventBegin(string aEventID) {
            LogEventBegin(aEventID, null);
        }

        /// <summary>
        /// Log the start of an event with a specfic ID and a single parameter set
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameterID">ID of the parameter set</param>
        /// <param name="aParameterValue">Value of the parameter set</param>
        public void LogEventBegin(string aEventID, string aParameterID, string aParameterValue) {
            LogEventBegin(aEventID, new Dictionary<string, string>() { { aParameterID, aParameterValue } });
        }

        /// <summary>
        /// Log the start of an event with a specfic ID with a dictionary of parameters
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameters">Dictionary containing all the parameters</param>
        public void LogEventBegin(string aEventID, Dictionary<string, string> aParameters) {
            if (aParameters != null) {
                Flurry.Instance.BeginLogEvent(aEventID, aParameters);
            } else {
                Flurry.Instance.BeginLogEvent(aEventID);
            }
        }

        /// <summary>
        /// Log the end of an event with a specfic ID
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        public void LogEventEnd(string aEventID) {
            LogEventEnd(aEventID, null);
        }

        /// <summary>
        /// Log the end of an event with a specfic ID and a single parameter set
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameterID">ID of the parameter set</param>
        /// <param name="aParameterValue">Value of the parameter set</param>
        public void LogEventEnd(string aEventID, string aParameterID, string aParameterValue) {
            LogEventEnd(aEventID, new Dictionary<string, string>() { { aParameterID, aParameterValue } });
        }

        /// <summary>
        /// Log the end of an event with a specfic ID with a dictionary of parameters
        /// </summary>
        /// <param name="aEventID">ID of the event</param>
        /// <param name="aParameters">Dictionary containing all the parameters</param>
        public void LogEventEnd(string aEventID, Dictionary<string, string> aParameters) {
            if (aParameters != null) {
                Flurry.Instance.EndLogEvent(aEventID, aParameters);
            } else {
                Flurry.Instance.EndLogEvent(aEventID);
            }
        }
        #endregion

    }

}
#endif