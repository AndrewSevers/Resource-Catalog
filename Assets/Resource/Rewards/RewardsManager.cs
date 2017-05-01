using Resource.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resource.Rewards {

    public class RewardsManager : Singleton<RewardsManager> {
        [Header("Login Rewards")]
        [SerializeField, Tooltip("Only give rewards for the exact matched days that are logged in.\n\nIf the user logs in on Monday and Wednesday then they will miss Tuesday's rewards.")]
        private bool matchDay;

        [Header("Rewards File")]
        [SerializeField, Tooltip("If this is checked then the rewards to be given must be provided by a file on the server")]
        private bool fileOnServer = false;
        [SerializeField, Visibility("fileOnServer", false), Tooltip("Path to file either locally (resources) or on a server (url + file path)")]
        private TextAsset rewardsFile;
        [SerializeField, Visibility("fileOnServer", true), Tooltip("Path to file either locally (resources) or on a server (url + file path)")]
        private string url;
        [SerializeField, Visibility("fileOnServer", true)]
        private string username;
        [SerializeField, Visibility("fileOnServer", true)]
        private string password;

        [SerializeField, ReadOnly]
        private RewardsData rewardsData;

        private bool initialized = false;

        #region Getters & Setters
        public RewardsData Data {
            get { return rewardsData; }
        }
        #endregion

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (initialized == false) {
                StartCoroutine(ProcessInitialization());
            }

            initialized = true;
        }

        private IEnumerator ProcessInitialization() {
            string json = null;

            if (fileOnServer) {
                WWWForm form = new WWWForm();
                form.AddField("user", username);
                form.AddField("password", password);

                WWW www = new WWW(url, form);

                yield return www;

                json = www.text;
            } else {
                json = rewardsFile.text;
            }

            rewardsData = JsonUtility.FromJson<RewardsData>(json);
        }
        #endregion
    }

    /// <summary>
    /// Reward Type is unique to each game
    /// </summary>
    public enum RewardType {
        SharkPoints = 0,
        Clams = 1
    }

    #region Login Rewards UI Class
    [RequireComponent(typeof(Animator))]
    public class LoginRewardsUI : MonoBehaviour {
        protected Animator animator;
        protected bool initialized = false;

        #region Getters & Setters
        public virtual GameObject Root {
            get { return gameObject; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            if (initialized == false) {
                animator = GetComponent<Animator>();
            }

            initialized = true;
        }
        #endregion

        #region Display
        public virtual void Open() {
            animator.SetTrigger("Open");
        }

        public virtual void Close() {
            animator.SetTrigger("Close");
        }
        #endregion

    }
    #endregion

    #region Reward Data Class
    [System.Serializable]
    public class RewardsData : ISerializationCallbackReceiver {
        [SerializeField]
        private RewardGroup[] login;
        [SerializeField]
        private RewardGroup[] daily;
        [SerializeField]
        private RewardGroup[] levels;

        private Dictionary<string, RewardGroup> rewardsMap;

        #region Getters & Setters
        public RewardGroup[] Login {
            get { return login; }
        }

        public RewardGroup[] Levels {
            get { return levels; }
        }
        #endregion

        #region Serialization
        public void OnAfterDeserialize() {
            // Setup dictionarys for game use
            rewardsMap = new Dictionary<string, RewardGroup>();
            foreach (RewardGroup data in levels) {
                rewardsMap[data.ID] = data;
            }

            foreach (RewardGroup data in login) {
                rewardsMap[data.ID] = data;
            }

            foreach (RewardGroup data in daily) {
                rewardsMap[data.ID] = data;
            }
        }

        public void OnBeforeSerialize() { }
        #endregion

        #region Utilities
        public RewardGroup GetRewardGroup(string aGroupID) {
            RewardGroup group;
            rewardsMap.TryGetValue(aGroupID, out group);
            return group;
        }

        public Reward GetReward(string aGroupID, string aRewardID = null) {
            RewardGroup group;
            rewardsMap.TryGetValue(aGroupID, out group);

            if (group != null) {
                return (string.IsNullOrEmpty(aRewardID) == false) ? group.GetReward(aRewardID) : group.GetReward();
            } else {
                return null;
            }
        }
        #endregion
    }
    #endregion

    #region Reward Group Class
    /// <summary>
    /// Reward with a set value
    /// </summary>
    [System.Serializable]
    public class RewardGroup : ISerializationCallbackReceiver {
        [SerializeField]
        private string id;
        [SerializeField]
        private Reward[] rewards;

        private Dictionary<string, Reward> rewardsMap;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }
        #endregion

        #region Serialization
        public void OnAfterDeserialize() {
            // Setup dictionarys for game use
            rewardsMap = new Dictionary<string, Reward>();
            foreach (Reward data in rewards) {
                rewardsMap[data.ID] = data;
            }
        }

        public void OnBeforeSerialize() { }
        #endregion

        #region Utilities
        /// <summary>
        /// Get a reward within the group specified by the given ID
        /// </summary>
        /// <param name="aID">ID of the reward to get</param>
        public Reward GetReward(string aID) {
            Reward rewards;
            rewardsMap.TryGetValue(aID, out rewards);
            return rewards;
        }

        /// <summary>
        /// Get a random reward with the group
        /// </summary>
        public Reward GetReward() {
            return rewards[Random.Range(0, rewards.Length)];
        }
        #endregion
    }
    #endregion

    #region Reward Class
    /// <summary>
    /// Set of rewards
    /// </summary>
    [System.Serializable]
    public class Reward {
        [SerializeField]
        private string id;
        [SerializeField]
        private string title;
        [SerializeField]
        private bool repeatable;
        [SerializeField]
        private bool useWeights;
        [SerializeField]
        private RewardValue[] values;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }

        public string Title {
            get { return title; }
        }

        public bool Repeatable {
            get { return repeatable; }
        }

        public RewardValue[] Values {
            get { return values; }
        }
        #endregion

        #region Utilites
        /// <summary>
        /// Get a random reward value
        /// </summary>
        /// <param name="aEnforceWeights">Whether or not to force the use of the weighted values of the reward</param>
        public RewardValue GetValue(bool aEnforceWeights = false) {
            if (useWeights || aEnforceWeights) {
                // Setup total weight value
                float totalWeight = 0;
                for (int i = 0; i < values.Length; i++) {
                    totalWeight += values[i].Weight;
                }

                // Select a random value from all the weights and start with a weight of zero
                float randomWeight = Random.Range(0, totalWeight);
                float currentWeight = 0;

                // Check each value to see if it matches the specified weight
                int index = 0;
                for (index = 0; index < values.Length; index++) {
                    currentWeight += values[index].Weight;
                    if (currentWeight > randomWeight) {
                        break;
                    }
                }

                return values[index];
            } else {
                return values[Random.Range(0, values.Length)];
            }
        }
        #endregion
    }
    #endregion

    #region Reward Value Class
    [System.Serializable]
    public class RewardValue {
        [SerializeField]
        private RewardType type;
        [SerializeField]
        private float weight;
        [SerializeField]
        private float amount;

        #region Getters & Setters
        public RewardType Type {
            get { return type; }
        }

        public float Weight {
            get { return weight; }
        }

        public float Amount {
            get { return amount; }
        }

        public int AmountToInt {
            get { return (int) amount; }
        }
        #endregion
    }
    #endregion

    #region Reward Info Class
    [System.Serializable]
    public class RewardInfo {
        [SerializeField]
        private string groupId;
        [SerializeField]
        private string rewardId;
        [SerializeField]
        private string uniqueId;

        #region Getters & Setters
        public string GroupID {
            get { return groupId; }
        }

        public string RewardID {
            get { return rewardId; }
        }

        public string UniqueID {
            get { return uniqueId; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new RewardInfo
        /// </summary>
        /// <param name="aGroupID">Id for the group this reward's info is attached to</param>
        /// <param name="aRewardID">Id for the reward this reward's info is attached to</param>
        /// <param name="aUniqueID">Id to uniquely identify this reward's info</param>
        public RewardInfo(string aGroupID, string aRewardID, string aUniqueID = null) {
            groupId = aGroupID;
            rewardId = aRewardID;
            uniqueId = aUniqueID;
        }
        #endregion

        #region Equals
        public override bool Equals(object aObject) {
            if (aObject is RewardInfo) {
                RewardInfo otherInfo = aObject as RewardInfo;

                 if (groupId.Equals(otherInfo.GroupID) == false) {
                    return false;
                } else if (rewardId.Equals(otherInfo.RewardID) == false) {
                    return false;
                } else if (uniqueId.Equals(otherInfo.UniqueID) == false) {
                    return false;
                }

                return true;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
        #endregion
    }
    #endregion

}
