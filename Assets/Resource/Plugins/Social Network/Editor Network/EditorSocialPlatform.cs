using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Resource.Plugins {

    public class EditorSocialPlatform : ISocialPlatform {
        private static EditorSocialPlatform instance;
        private static EditorSocialUser mLocalUser = null;

        #region Getters & Setters
        public ILocalUser localUser {
            get { return mLocalUser; }
        }

        public EditorSocialPlatform Instance {
            get {
                if (instance == null) {
                    instance = new EditorSocialPlatform();
                }

                return instance;
            }
        }
        #endregion

        public static EditorSocialPlatform Activate() {
            Social.Active = instance;

            mLocalUser = new EditorSocialUser();

            return instance;
        }

        public void Authenticate(ILocalUser user, Action<bool, string> callback) {
            callback(true, null);
        }

        public void Authenticate(ILocalUser user, Action<bool> callback) {
            callback(true);
        }

        public IAchievement CreateAchievement() {
            throw new NotImplementedException();
        }

        public ILeaderboard CreateLeaderboard() {
            throw new NotImplementedException();
        }

        public bool GetLoading(ILeaderboard board) {
            throw new NotImplementedException();
        }

        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback) {
            throw new NotImplementedException();
        }

        public void LoadAchievements(Action<IAchievement[]> callback) {
            throw new NotImplementedException();
        }

        public void LoadFriends(ILocalUser user, Action<bool> callback) {
            throw new NotImplementedException();
        }

        public void LoadScores(ILeaderboard board, Action<bool> callback) {
            throw new NotImplementedException();
        }

        public void LoadScores(string leaderboardID, Action<IScore[]> callback) {
            throw new NotImplementedException();
        }

        public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback) {
            throw new NotImplementedException();
        }

        public void ReportProgress(string achievementID, double progress, Action<bool> callback) {
            throw new NotImplementedException();
        }

        public void ReportScore(long score, string board, Action<bool> callback) {
            throw new NotImplementedException();
        }

        public void ShowAchievementsUI() {
            throw new NotImplementedException();
        }

        public void ShowLeaderboardUI() {
            throw new NotImplementedException();
        }
    }

}

