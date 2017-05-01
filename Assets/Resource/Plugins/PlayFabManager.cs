#if PlayFab
using GooglePlayGames;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Resource.Data {

    public class PlayFabManager : Singleton<PlayFabManager> {
        private PlayFabUser user;

        #region Events
        public delegate void OnLoggedInEvent(bool aSuccess);
        public OnLoggedInEvent OnLoginComplete;
        public delegate void OnDownloadEvent(bool aSuccess);
        public OnDownloadEvent OnDownloadComplete;
        public delegate void OnUploadEvent(bool aSuccess);
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
        public void Login(bool aCreate = false, Service aService = Service.Device) {
#if UNITY_EDITOR
            LoginWithCustomIDRequest customRequest = new LoginWithCustomIDRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, CustomId = "Test User" };
            PlayFabClientAPI.LoginWithCustomID(customRequest, LoginSuccess, LoginFailure);
#elif UNITY_IOS
            LoginWithGameCenterRequest gameCenterRequest = new LoginWithGameCenterRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, PlayerId = Social.localUser.id };                    
            PlayFabClientAPI.LoginWithGameCenter(gameCenterRequest, LoginSuccess, LoginFailure);
#elif UNITY_ANDROID
            LoginWithGoogleAccountRequest googleAccountRequest = new LoginWithGoogleAccountRequest() { TitleId = PlayFabSettings.TitleId, CreateAccount = aCreate, ServerAuthCode = (Social.Active as PlayGamesPlatform).GetServerAuthCode() };
            PlayFabClientAPI.LoginWithGoogleAccount(googleAccountRequest, LoginSuccess, LoginFailure);
#endif
        }

        private void LoginSuccess(LoginResult aResult) {
            user = new PlayFabUser(aResult.PlayFabId);

            if (OnLoginComplete != null) {
                OnLoginComplete(true);
            }
        }

        private void LoginFailure(PlayFabError aError) {
            if (OnLoginComplete != null) {
                OnLoginComplete(false);
            }
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
        public void Upload(string aJsonData) {
            UpdateUserDataRequest request = new UpdateUserDataRequest() { };
            PlayFabClientAPI.UpdateUserData(request, UploadSuccess, UploadFaliure);
        }

        private void UploadSuccess(UpdateUserDataResult aResult) {
        }

        private void UploadFaliure(PlayFabError aError) {

        }
        #endregion

        #region Download
        public void Download() {
            GetUserDataRequest request = new GetUserDataRequest() { PlayFabId = user.ID };
            PlayFabClientAPI.GetUserData(request, DownloadSuccess, DownloadFailure);
        }

        private void DownloadSuccess(GetUserDataResult aResult) {
            
        }

        private void DownloadFailure(PlayFabError aError) {

        }
        #endregion

        #region Time
        public void RequestServerTime() {
            PlayFab.ServerModels.GetTimeRequest timeRequest = new PlayFab.ServerModels.GetTimeRequest();
            PlayFabServerAPI.GetTime(timeRequest, GetTimeSuccess, GetTimeFailure);
        }

        private void GetTimeSuccess(PlayFab.ServerModels.GetTimeResult aResult) {

        }

        private void GetTimeFailure(PlayFabError aError) {

        }
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
        GameCenter,
        GooglePlay,
        Facebook,
        Twitch
    }

}

#endif
