using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Resource.Plugins {

    public class EditorSocialUser : ILocalUser {
        public bool authenticated {
            get { return true; }
        }

        public IUserProfile[] friends {
            get {
                throw new NotImplementedException();
            }
        }

        public string id {
            get { return "1"; }
        }

        public Texture2D image {
            get {
                throw new NotImplementedException();
            }
        }

        public bool isFriend {
            get { return true; }
        }

        public UserState state {
            get { return UserState.Online; }
        }

        public bool underage {
            get { return false; }
        }

        public string userName {
            get { return "Test User"; }
        }

        public void Authenticate(Action<bool, string> callback) {
            callback(true, null);
        }

        public void Authenticate(Action<bool> callback) {
            callback(true);
        }

        public void LoadFriends(Action<bool> callback) {
            throw new NotImplementedException();
        }
    }

}
