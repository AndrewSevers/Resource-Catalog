using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace Extensions.Plugins {

    /// <summary>
    /// Manager of social connections for systems such as GameCenter (iOS) and Google Play Game Services (Android)
    /// </summary>
    public class SocialManager : Singleton<SocialManager> {
        #region Events
        public delegate void OnAuthenticatedEvent(bool aSuccess);
        public OnAuthenticatedEvent OnAuthenticated;
        #endregion

        #region Authentication
        public void Authenticate() {
#if UNITY_EDITOR
            EditorSocialPlatform.Activate();
#elif UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().AddOauthScope("profile").RequestServerAuthCode(false).Build();
            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.Activate();
#elif UNITY_IOS
            // IOS is Unity's Social default
#endif

            Social.localUser.Authenticate(ProcessAuthentication);
        }

        private void ProcessAuthentication(bool aSuccess, string aDebug) {
            if (aSuccess) {
                Debug.Log("[SocialManager] Authenticated" + " : " + aDebug);
            } else {
                Debug.LogError("[SocialManager] Not authenticated" + " : " + aDebug);
            }

            if (OnAuthenticated != null) {
                OnAuthenticated(aSuccess);
            }
        }
        #endregion

    }

}
