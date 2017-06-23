using Extensions.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ADS
namespace Extensions {

    public class AdManager : Singleton<AdManager> {
        [SerializeField, Tooltip("Id of app within Vungle for ios")]
        private string iosAppId;
        [SerializeField, Tooltip("Id of app within Vungle for android")]
        private string androidAppId;
        [SerializeField, Tooltip("Overlay that will block all raycasts (to disable multiple presses)")]
        private Canvas overlayCanvas;

        private bool initialized = false;
        private bool adPlaying = false;

        #region Events
        public delegate void OnVideoAdStartedEvent();
        public OnVideoAdStartedEvent OnVideoAdStarted;

        public delegate void OnVideoAdFinishedEvent(bool aCompleted);
        public OnVideoAdFinishedEvent OnVideoAdEnded;

        public delegate void OnVideoAdAvailableEvent(bool aAvailable);
        public OnVideoAdAvailableEvent OnVideoAdAvailable;
        #endregion

        #region Getters & Setters
        public bool IsAdAvailable {
            get { return Vungle.isAdvertAvailable(); }
        }

        public bool IsAdPlaying {
            get { return adPlaying; }
        }
        #endregion

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (initialized == false) {
                Vungle.onAdStartedEvent += OnVideoStart;
                Vungle.onAdFinishedEvent += OnVideoFinish;
                Vungle.adPlayableEvent += OnVideoBecameAvailable;
            }

            Vungle.init(androidAppId, iosAppId);

            overlayCanvas.gameObject.SetActive(false);

            initialized = true;
        }
        #endregion

        #region Video Management
        /// <summary>
        /// Play video ad with default (common) options
        /// </summary>
        /// <param name="aIncentivized">Should the video be incentivized (notified to server)</param>
        /// <param name="aUserID">User ID to notify the server with for the incentivized ad</param>
        /// <param name="aOrientation">Orientation of the ad</param>
        public void PlayVideo(bool aIncentivized = false, string aUserID = "", int aOrientation = 6, bool aWaitForPlayable = true) {
#if UNITY_EDITOR
            AdFinishedEventArgs args = new AdFinishedEventArgs();
            args.IsCompletedView = true;

            OnVideoFinish(args);
#else
            if (Vungle.isAdvertAvailable()) {
                overlayCanvas.gameObject.SetActive(true);

                Vungle.playAd(aIncentivized, aUserID, aOrientation);
            } else {
                if (aWaitForPlayable) {
                    StartCoroutine(PlayOnceAvailable(aIncentivized, aUserID, aOrientation));
                } else {
                    Debug.LogWarning("[AdManager] No ad available");
                }
            }
#endif
        }

        /// <summary>Play video ad with non-required options</summary>
        /// <param name="aOptions">Options to play the video with</param>
        public void PlayVideo(params KeyValuePair<string, object>[] aOptions) {
            Dictionary<string, object> options = new Dictionary<string, object>();
            for (int i = 0; i < aOptions.Length; i++) {
                options[aOptions[i].Key] = aOptions[i].Value;
            }

            PlayVideo(options);
        }

        /// <summary>Play video ad with options</summary>
        /// <param name="aOptions">Options to play the video with</param>
        public void PlayVideo(Dictionary<string, object> aOptions, bool aWaitForPlayable = true) {
#if UNITY_EDITOR
            AdFinishedEventArgs args = new AdFinishedEventArgs();
            args.IsCompletedView = true;

            OnVideoFinish(args);
#else
            if (Vungle.isAdvertAvailable()) {
                overlayCanvas.gameObject.SetActive(true);

                Vungle.playAdWithOptions(aOptions);
            } else {
                if (aWaitForPlayable) {
                    StartCoroutine(PlayOnceAvailable(aOptions));
                } else {
                    Debug.LogWarning("[AdManager] No ad available");
                }
            }
#endif
        }

        private void OnVideoStart() {
            Debug.Log("[Ad Manager] Video started");

            if (OnVideoAdStarted != null) {
                OnVideoAdStarted();
            }

            adPlaying = true;
        }

        private void OnVideoFinish(AdFinishedEventArgs aArgs) {
            Debug.Log("[Ad Manager] Video finished");

            if (OnVideoAdEnded != null) {
                OnVideoAdEnded(aArgs.IsCompletedView);
            }

            overlayCanvas.gameObject.SetActive(false);
            adPlaying = false;

            Initialize();
        }

        private void OnVideoBecameAvailable(bool aAvailable) {
            Debug.Log("[Ad Manager] Video available");

            if (OnVideoAdAvailable != null) {
                OnVideoAdAvailable(aAvailable);
            }
        }
        #endregion

        #region Application Status
        private void OnApplicationPause(bool aPause) {
            if (aPause) {
                Vungle.onPause();
            } else {
                Vungle.onResume();
            }
        }
        #endregion

        #region Utilities
        private IEnumerator PlayOnceAvailable(bool aIncentivized = false, string aUserID = "", int aOrientation = 6) {
            overlayCanvas.gameObject.SetActive(true);

            while (Vungle.isAdvertAvailable() == false) {
                yield return null;
            }

            Vungle.playAd(aIncentivized, aUserID, aOrientation);
        }

        private IEnumerator PlayOnceAvailable(Dictionary<string, object> aOptions) {
            overlayCanvas.gameObject.SetActive(true);

            while (Vungle.isAdvertAvailable() == false) {
                yield return null;
            }

            Vungle.playAdWithOptions(aOptions);
        }
        #endregion

    }

    public enum AdStatus {
        Enabled = 0,
        Disabled = 1,
        Active = 2,
        Inactive = 3
    }

}
#endif
