using Extensions.Properties;
using System.Collections;
using UnityEngine;

namespace Extensions {

    public class InternetConnectivityManager : Singleton<InternetConnectivityManager> {
        [SerializeField, AssetPath]
        private string processingUIPath;
        [SerializeField, LimitedRange]
        private LimitedRange connectionDuration;

        private ProcessingElement processingUI;
        private bool isConnected;
        private bool skippable;

        #region Events
        public delegate void OnTestCompleteEvent(IConnectivityResult aStatus);
        public OnTestCompleteEvent OnTestComplete;
        #endregion

        #region Getters & Setters
        public ProcessingElement Dialog {
            get {
                if (processingUI == null) {
                    processingUI = Instantiate(Resources.Load<ProcessingElement>(processingUIPath));
                }

                return processingUI;
            }
        }

        public LimitedRange ConnectionDuration {
            get { return connectionDuration; }
        }

        public bool IsConnected {
            get { return isConnected; }
        }

        public bool Skippable {
            get { return skippable; }
            set { skippable = value; }
        }
        #endregion

        #region Connection UI
        public void OpenDialog() {
            Dialog.DisplayProcessing("Connecting");
        }

        public void CloseDialog() {
            Dialog.Close();
        }

        public void OpenErrorDialog(string aText, bool aForceSkippable = false) {
            Dialog.DisplayError(aText, skippable || aForceSkippable);
        }

        public void OpenErrorDialog(bool aForceSkippable = false) {
            Dialog.DisplayError(skippable || aForceSkippable);
        }
        #endregion

        #region Testing
        /// <summary>
        /// Check to see if the current device has a working connection to the internet. Subscribe to 'OnTestComplete' to be informed of the results.
        /// </summary>
        /// <param name="aDisplayUI">Whether or not to display UI</param>
        /// <param name="aUrl">Url to test</param>
        /// <param name="aTestComplete">Test complete handler</param>
        public void TestConnect(bool aOpenUI, string aUrl = "https://google.com", bool aSkippable = false) {
            if (aOpenUI) {
                OpenDialog();
            }

            skippable = aSkippable;

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
        private ConnectivityResult DetermineResult(string aPossibleError) {
            return new ConnectivityResult(string.IsNullOrEmpty(aPossibleError) == false, aPossibleError);
        }
        #endregion

    }

}
