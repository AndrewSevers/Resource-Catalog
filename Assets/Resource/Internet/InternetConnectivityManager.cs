using Resource.Properties;
using Resource.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Resource {

    public class InternetConnectivityManager : Singleton<InternetConnectivityManager> {
        [SerializeField]
        private ConnectingUI connectingUI;
        [SerializeField, LimitedRange]
        private LimitedRange connectionDuration;
        
        private bool isConnected;

        #region Events
        public delegate void OnTestCompleteEvent(IConnectivityResult aStatus);
        public OnTestCompleteEvent OnTestComplete;

        public delegate void OnReconnectEvent();
        public OnReconnectEvent OnReconnectSelected;

        public delegate void OnContinueEvent();
        public OnContinueEvent OnContinueSelected;
        #endregion

        #region Getters & Setters
        public ConnectingUI Dialog {
            get { return connectingUI; }
        }

        public LimitedRange ConnectionDuration {
            get { return connectionDuration; }
        }

        public bool IsConnected {
            get { return isConnected; }
        }
        #endregion

        #region Connection UI
        public void OpenDialog() {
            connectingUI.DisplayConnecting();
        }

        public void CloseDialog() {
            connectingUI.Close();
        }
        #endregion

        #region Testing
        /// <summary>
        /// Check to see if the current device has a working connection to the internet. Subscribe to 'OnTestComplete' to be informed of the results.
        /// </summary>
        /// <param name="aDisplayUI">Whether or not to display UI</param>
        /// <param name="aUrl">Url to test</param>
        /// <param name="aTestComplete">Test complete handler</param>
        public void TestConnect(bool aOpenUI, string aUrl = "https://google.com") {
            if (aOpenUI) {
                OpenDialog();
            }

            StartCoroutine(ProcessConnectionTest(aUrl));
        }

        private IEnumerator ProcessConnectionTest(string aUrl = "https://google.com", bool aRetry = false) {
            // Wait any additional time if we don't want this check to flash
            yield return new WaitForSeconds(connectionDuration.Min);

            WWW wwwRequest = new WWW(aUrl); 
            yield return wwwRequest;

            // Check the result and process its response (either with UI or just a callback)
            ConnectivityResult result = DetermineResult(wwwRequest.error);

            isConnected = !result.Error;
            CompleteTest(result);
        }

        private void CompleteTest(ConnectivityResult aResult) {
            if (OnTestComplete != null) {
                OnTestComplete(aResult);
            }
        }
        #endregion

        #region Utilities
        public void Reconnect() {
            Dialog.Reconnect();

            if (OnReconnectSelected != null) {
                OnReconnectSelected();
            }
        }

        public void Continue() {
            Dialog.Close();

            if (OnContinueSelected != null) {
                OnContinueSelected();
            }
        }

        private ConnectivityResult DetermineResult(string aPossibleError) {
            return new ConnectivityResult(string.IsNullOrEmpty(aPossibleError) == false, aPossibleError);
        }
        #endregion

    }

}
