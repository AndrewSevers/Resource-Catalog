#if Facebook
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resource {

    public class FacebookManager : Singleton<FacebookManager> {
        #region Events
        public delegate void OnInitFinishedEvent();
        public OnInitFinishedEvent OnInitFinished;

        public delegate void OnLoginFinishEvent(ILoginResult aResult);
        public OnLoginFinishEvent OnLoginFinish;

        public delegate void OnShareFinishEvent(IShareResult aResult);
        public OnShareFinishEvent OnShareFinish;

        public delegate void OnUploadPhotoFinishEvent(IGraphResult aResult);
        public OnUploadPhotoFinishEvent OnUploadPhotoFinish;

        public delegate void OnSharePhotoFinishEvent(IShareResult aResult);
        public OnSharePhotoFinishEvent OnSharePhotoFinish;

        public delegate void OnFailureEvent(Result aReseult);
        public OnFailureEvent OnFailure;
        #endregion

        #region Result Struct
        public struct Result {
            private string description;
            private Code errorCode;

            public enum Code {
                None = 0,
                Init = 1,
                Login = 2,
                Share = 3,
                Upload = 4,
                Retrieve = 5
            }

            #region Getters & Setters
            public string Description {
                get { return description; }
            }

            public bool IsError {
                get { return errorCode != Code.None; }
            }

            public Code ErrorCode {
                get { return errorCode; }
            }
            #endregion

            #region Constructor
            /// <summary>Generate a result with given description and optional error code</summary>
            /// <param name="aDescription">Description of result</param>
            /// <param name="aErrorCode">Code relating to specific error</param>
            public Result(string aDescription, Code aErrorCode = Code.None) {
                description = aDescription;
                errorCode = aErrorCode;
            }
            #endregion
        }
        #endregion

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (FB.IsInitialized == false) {
                FB.Init(OnInitCompleted, OnHideUnity);
            } else {
                OnInitCompleted();
            }
        }

        private void OnInitCompleted() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else if (OnFailure != null) {
                Debug.LogWarning("[FacebookManager] Failed to initialize");
                OnFailure(new Result("Facebook failed to initialize", Result.Code.Init));
            }

            if (OnInitFinished != null) {
                OnInitFinished();
            }
        }

        private void OnHideUnity(bool aIsGameShown) {
            if (aIsGameShown) {
                Time.timeScale = 1;
            } else {
                Time.timeScale = 0;
            }
        }
        #endregion

        #region Login
        public void Login(bool aReadOnly = true, params string[] aPermissions) {
            if (FB.IsLoggedIn == false || HasPermissions(aPermissions) == false) {
                if (aReadOnly) {
                    if (aPermissions != null && aPermissions.Length > 0) {
                        FB.LogInWithReadPermissions(aPermissions, OnLoginCompleted);
                    } else {
                        FB.LogInWithReadPermissions(callback: OnLoginCompleted);
                    }
                } else {
                    if (aPermissions != null && aPermissions.Length > 0) {
                        FB.LogInWithPublishPermissions(aPermissions, OnLoginCompleted);
                    } else {
                        FB.LogInWithPublishPermissions(callback: OnLoginCompleted);
                    }
                }
            } else {
                if (OnLoginFinish != null) {
                    OnLoginFinish(new LoginResult(new ResultContainer(string.Empty)));
                }

                Debug.LogWarning("[FacebookManager] Already logged in");
            }
        }

        private void OnLoginCompleted(ILoginResult aResult) {
            if (FB.IsLoggedIn) {
                Debug.Log("[FacebookManager] Logged In");
            } else {
                Debug.Log("FacebookManager] Not Logged In");
            }

            if (OnLoginFinish != null) {
                OnLoginFinish(aResult);
            }
        }
        #endregion

        #region Photo Management
        public void UploadPhoto(string aPhotoLocation = null) {
            StartCoroutine(ProcessUploadingPhoto(aPhotoLocation));
        }

        private IEnumerator ProcessUploadingPhoto(string aPhotoLocation) {
            WWWForm form = new WWWForm();

            yield return new WaitForEndOfFrame();

            // Photo location is null take a screenshot
            if (string.IsNullOrEmpty(aPhotoLocation)) {
                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                byte[] screenshot = texture.EncodeToPNG();

                form.AddBinaryData("image", screenshot, "sharkweek_shark_dash_score.png");
            // Upload photo from phones storage
            } else {
                // TODO fill in
            }

            FB.API("me/photos", HttpMethod.POST, OnUploadFinish, form);
        }

        private void OnUploadFinish(IGraphResult aResult) {
            if (string.IsNullOrEmpty(aResult.Error) == false) {
                Debug.LogWarning("[FacebookManager] Upload errored: " + aResult.Error);
            }

            if (OnUploadPhotoFinish != null) {
                OnUploadPhotoFinish(aResult);
            }
        }
        #endregion

        #region Sharing To Feed
        public void ShareLink(string aTitle = null, string aDescription = null, System.Uri aContent = null, System.Uri aPhoto = null) {
            FB.ShareLink(aContent, aTitle, aDescription, aPhoto, OnShareCompleted);
        }

        public void ShareFeed(string aToID = "", System.Uri aLink = null, string aLinkName = null, string aLinkCaption = null, string aLinkDescription = null, string aMediaSource = null, System.Uri aPhoto = null) {
            FB.FeedShare(aToID, aLink, aLinkName, aLinkCaption, aLinkDescription, aPhoto, aMediaSource, OnShareCompleted);
        }

        private void OnShareCompleted(IShareResult aResult) {
            if (string.IsNullOrEmpty(aResult.Error) == false) {
                Debug.LogWarning("[FacebookManager] Share errored: " + aResult.Error);
            } else {
                Debug.LogWarning("[FacebookManager] Share success");
            }

            if (OnShareFinish != null) {
                OnShareFinish(aResult);
            }
        }
        #endregion

        #region Utilities
        public bool HasPermissions(params string[] aPermissions) {
            if (aPermissions != null && aPermissions.Length > 0) {
                List<string> currentPermissions = new List<string>(AccessToken.CurrentAccessToken.Permissions);

                for (int i = 0; i < aPermissions.Length; i++) {
                    if (currentPermissions.Contains(aPermissions[i]) == false) {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

    }

}
#endif
