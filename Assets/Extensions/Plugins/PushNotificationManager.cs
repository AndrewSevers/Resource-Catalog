#if PUSH
using Extensions.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Plugins {

    public class PushNotificationManager : Singleton<PushNotificationManager> {
        [SerializeField]
        private string appCode;
        [SerializeField]
        private string googleProjectNumber;

        [Header("Interface")]
        [SerializeField]
        private PopupElement registrationPopup;

        private bool initialized = false;

        #region Events
        public delegate void OnRegistrationCompleteEvent(RegistrationStatus aStatus);
        public OnRegistrationCompleteEvent OnRegistrationComplete;
        #endregion

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (initialized == false) {
                Pushwoosh.ApplicationCode = appCode;
                Pushwoosh.GcmProjectNumber = googleProjectNumber;

                Pushwoosh.Instance.OnRegisteredForPushNotifications += OnRegisteredForPushNotifications;
                Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += OnFailedToRegisteredForPushNotifications;
                Pushwoosh.Instance.OnPushNotificationsReceived += OnPushNotificationsReceived;
            }

            initialized = true;
        }
        #endregion

        #region Registration
        public void OpenRegistrationUI() {
            registrationPopup.Open();
        }

        public void Register() {
            if (OnRegistrationComplete != null) {
                OnRegistrationComplete(RegistrationStatus.Accepted);
            }

            Pushwoosh.Instance.RegisterForPushNotifications();

            registrationPopup.Close();
        }

        public void DeclineRegistration() {
            if (OnRegistrationComplete != null) {
                OnRegistrationComplete(RegistrationStatus.Declined);
            }

            registrationPopup.Close();
        }
        #endregion

        #region Events Handling
        private void OnRegisteredForPushNotifications(string aToken) {
            Debug.Log("[PushNotificationManager] Registered: " + aToken);
        }

        private void OnFailedToRegisteredForPushNotifications(string aError) {
            Debug.Log("[PushNotificationManager] Failed to register: " + aError);
        }

        private void OnPushNotificationsReceived(string aPayload) {
            Debug.Log("[PushNotificationManager] Notification: " + aPayload);
        }
        #endregion
    }

    public enum RegistrationStatus {
        None,
        Accepted,
        Declined
    }

}
#endif