using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace Resource.Plugins {

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
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().AddOauthScope("profile").Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
#elif UNITY_IOS
            // IOS is Unity's Social default
#endif

            Social.localUser.Authenticate(ProcessAuthentication);
        }

        private void ProcessAuthentication(bool aSuccess, string aString) {
            if (aSuccess) {
                Debug.Log("[SocialManager] Authenticated - " + aString);
            } else {
                if (Social.localUser.authenticated) {
                    aSuccess = true;
                    Debug.LogError("[SocialManager] IS ACTUALLY AUTHENTICATED");
                }

                Debug.LogError("[SocialManager] Not authenticated - " + aString);
            }

            if (OnAuthenticated != null) {
                OnAuthenticated(aSuccess);
            }
        }
        #endregion

    }

}
