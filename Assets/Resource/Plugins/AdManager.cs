#if Ads
using System.Collections.Generic;
using UnityEngine;

namespace Resource {

    public class AdManager : Singleton<AdManager> {
        [SerializeField, Tooltip("Id of app within Vungle for android")]
        private string iosAppId;
        [SerializeField, Tooltip("Id of app within Vungle for android")]
        private string androidAppId;
        
        private bool initialized = false;

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (initialized == false) {
                Vungle.init(androidAppId, iosAppId);
            }

            initialized = true;
        }
        #endregion

        #region Video Management
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
        public void PlayVideo(Dictionary<string, object> aOptions) {
            Vungle.playAdWithOptions(aOptions);
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

    }

    public enum AdStatus {
        Enabled = 0,
        Disabled = 1,
        Active = 2,
        Inactive = 3
    }

}

#endif