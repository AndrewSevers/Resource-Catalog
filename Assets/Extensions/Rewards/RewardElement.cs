using Extensions.Properties;
using Extensions.Rewards;
using Extensions.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardElement : MonoBehaviour {
    [SerializeField, Tooltip("Optional field that if set will get a specific reward group attached to this element")]
    private string rewardGroupId;
    [SerializeField, Tooltip("Optional field that if set will get this specific reward attached to this element"), Visibility("rewardGroupId", true, false)]
    private string rewardId;

    [Header("Reward Details")]
    [SerializeField]
    private Text header;
    [SerializeField, Tooltip("Icon for the reward (not the container)\n\nThis can be used for a reveal")]
    private Image icon;
    [SerializeField]
    private TextElement details;

    [Header("Icon Sprites")]
    [SerializeField]
    private Sprite clamSprite;
    [SerializeField]
    private Sprite sharkPointsSprite;

    public delegate void OnOpenHandler(RewardElement aRewardElement);
    public OnOpenHandler OnOpen;

    private Animator animator;
    private Button button;
    private Reward reward;
    private RewardValue value;
    private RewardElementStatus status;
    private bool initialized = false;

    public enum RewardElementStatus {
        Locked = 0,
        Closed = 1,
        Opened = 2
    }

    #region Getters & Setters
    public string RewardGroupID {
        get { return rewardGroupId; }
    }

    public string RewardID {
        get { return rewardId; }
    }

    public Image Icon {
        get { return icon; }
    }

    public Text Header {
        get { return header; }
    }

    public TextElement Details {
        get { return details; }
    }

    public Animator Animator {
        get { return animator; }
    }

    public RewardElementStatus Status {
        get { return status; }
        set { status = value; }
    }

    public Reward Reward {
        get { return reward; }
    }

    public RewardValue Value {
        get { return value; }
    }
    #endregion

    #region Initialization
    private void Start() {
        if (initialized == false) {
            if (string.IsNullOrEmpty(rewardGroupId) == false) {
                RewardGroup group = RewardsManager.Instance.Data.GetRewardGroup(rewardGroupId);
                if (group != null) {
                    if (string.IsNullOrEmpty(rewardId) == false) {
                        Initialize(group.GetReward(rewardId));
                    } else {
                        Initialize(group.GetReward());
                    }
                }
            } else {
                Initialize(null);
            }
        }
    }

    public void Initialize(Reward aReward, RewardElementStatus aStatus = RewardElementStatus.Locked) {
        if (initialized == false) {
            animator = GetComponent<Animator>();
            button = GetComponent<Button>();
        }

        if (aReward != null) {
            reward = aReward;
            value = reward.GetValue();

            header.text = reward.Title;
            details.UpdateText(value.Amount);

            switch (value.Type) {
                case RewardType.Clams:
                    icon.sprite = clamSprite;
                    break;
                case RewardType.SharkPoints:
                    icon.sprite = sharkPointsSprite;
                    break;
            }
        }

        status = aStatus;
        switch (status) {
            case RewardElementStatus.Locked:
                button.interactable = false;
                break;
            case RewardElementStatus.Closed:
                if (animator != null) {
                    animator.SetTrigger("Closed");
                }

                button.interactable = true;
                break;
            case RewardElementStatus.Opened:
                if (animator != null) {
                    animator.SetTrigger("Opened");
                }

                button.interactable = false;
                break;
        }

        initialized = true;
    }
    #endregion

    #region Button
    public void Open() {
        if (status == RewardElementStatus.Closed) {
            if (animator != null) {
                animator.SetTrigger("Open");
            }

            status = RewardElementStatus.Opened;
        }
    }
    #endregion

    #region Utilities
    private void OpeningFinished() {
        if (OnOpen != null) {
            OnOpen(this);
        }
    }
    #endregion

}
