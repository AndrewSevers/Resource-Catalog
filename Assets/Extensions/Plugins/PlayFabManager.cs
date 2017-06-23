#if PLAYFAB
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab.SharedModels;

#if UNITY_ANDROID
using GooglePlayGames;
#endif

namespace Extensions.Data {

    public class PlayFabManager : Singleton<PlayFabManager> {
        private PlayFabUser user;

        #region Events
        public delegate void OnLoggedInEvent(PlayFabResultCommon aResult);
        public OnLoggedInEvent OnLoginComplete;
        public delegate void OnDownloadEvent(PlayFabResultCommon aResult);
        public OnDownloadEvent OnDownloadComplete;
        public delegate void OnUploadEvent(PlayFabResultCommon aResult);
        public OnUploadEvent OnUploadComplete;
        #endregion

        #region Getters & Setters
        public bool IsLoggedIn {
            get { return PlayFabClientAPI.IsClientLoggedIn(); }
        }
        #endregion

        #region Login
        /// <summary>
        /// Login to PlayFab through provided service
        /// </summary>
        /// <param name="aCreate">If true the user will be registered into Playfab if it doens't exist</param>
        /// <param name="aService">Service to login with</param>
        public void Login(bool aCreate = false) {
            GetPlayerCombinedInfoRequestParams infoParameters = new GetPlayerCombinedInfoRequestParams() { GetUserData = true };

#if UNITY_EDITOR
            LoginWithCustomIDRequest customRequest = new LoginWithCustomIDRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, CustomId = "Test User", InfoRequestParameters = infoParameters };
            PlayFabClientAPI.LoginWithCustomID(customRequest, LoginSuccess, LoginFailure);
#elif UNITY_IOS
            LoginWithGameCenterRequest gameCenterRequest = new LoginWithGameCenterRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, PlayerId = Social.localUser.id, InfoRequestParameters = infoParameters  };                    
            PlayFabClientAPI.LoginWithGameCenter(gameCenterRequest, LoginSuccess, LoginFailure);
#elif UNITY_ANDROID
            Debug.Log("[PlayFabManager] Logging in: titleId = " + PlayFabSettings.TitleId + ", auth code = " + PlayGamesPlatform.Instance.GetServerAuthCode());

            LoginWithGoogleAccountRequest googleAccountRequest = new LoginWithGoogleAccountRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, ServerAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode(), InfoRequestParameters = infoParameters  };
            PlayFabClientAPI.LoginWithGoogleAccount(googleAccountRequest, LoginSuccess, LoginFailure);
#endif
        }

        private void LoginSuccess(LoginResult aResult) {
            user = new PlayFabUser(aResult.PlayFabId);

            if (OnLoginComplete != null) {
                OnLoginComplete(aResult);
            }
        }

        private void LoginFailure(PlayFabError aError) {
            if (OnLoginComplete != null) {
                OnLoginComplete(new PlayFabErrorCommon(aError));
            }

            Debug.Log("[PlayFabManager] Failed to login: code = " + aError.Error + ", " + aError.ErrorMessage);
        }
        #endregion

        #region Account Info
        public void GetAccountInfo() {
            GetAccountInfoRequest request = new GetAccountInfoRequest();
            PlayFabClientAPI.GetAccountInfo(request, GetAccountInfoSuccess, GetAccountInfoError);
        }

        private void GetAccountInfoSuccess(GetAccountInfoResult aResult) {

        }

        private void GetAccountInfoError(PlayFabError aError) {

        }
        #endregion

        #region Upload
        public void Upload(string aKey, string aData) {
            Dictionary<string, string> data = new Dictionary<string, string>() { { aKey, aData } };
            Upload(data);
        }

        public void Upload(Dictionary<string, string> aData) {
            if (user != null) {
                UpdateUserDataRequest request = new UpdateUserDataRequest() { Data = aData };
                PlayFabClientAPI.UpdateUserData(request, UploadSuccess, UploadFaliure);
            }
        }

        private void UploadSuccess(UpdateUserDataResult aResult) {
            if (OnUploadComplete != null) {
                OnUploadComplete(aResult);
            }
        }

        private void UploadFaliure(PlayFabError aError) {
            if (OnUploadComplete != null) {
                OnUploadComplete(new PlayFabErrorCommon(aError));
            }
        }
        #endregion

        #region Download
        public void Download() {
            if (user != null) {
                GetUserDataRequest request = new GetUserDataRequest() { PlayFabId = user.ID };
                PlayFabClientAPI.GetUserData(request, DownloadSuccess, DownloadFailure);
            }
        }

        private void DownloadSuccess(GetUserDataResult aResult) {
            if (OnDownloadComplete != null) {
                OnDownloadComplete(aResult);
            }
        }

        private void DownloadFailure(PlayFabError aError) {
            if (OnDownloadComplete != null) {
                OnDownloadComplete(new PlayFabErrorCommon(aError));
            }
        }
        #endregion

        #region Time
#if ENABLE_PLAYFABSERVER_API
        public void RequestServerTime() {
            PlayFab.ServerModels.GetTimeRequest timeRequest = new PlayFab.ServerModels.GetTimeRequest();
            PlayFabServerAPI.GetTime(timeRequest, GetTimeSuccess, GetTimeFailure);
        }

        private void GetTimeSuccess(PlayFab.ServerModels.GetTimeResult aResult) {

        }

        private void GetTimeFailure(PlayFabError aError) {

        }
#endif
        #endregion
    }

    public class PlayFabUser {
        private string id;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }
        #endregion

        #region Constructor
        public PlayFabUser(string aID) {
            id = aID;
        }
        #endregion
    }

    public enum Service {
        None,
        Disabled,
        Device,
        Custom,
        GameCenter,
        GooglePlay,
        Facebook,
        Twitch
    }

}
#endif