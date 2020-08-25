using UnityEngine;
using UnityEngine.UI;

namespace Extensions {

  [RequireComponent(typeof(Animator))]
  public class ProcessingElement : MonoBehaviour {
    [SerializeField, Tooltip("[Optional] If true the UI will display run the 'Success' animation trigger")]
    private bool hasSuccessMenu = false;
    [SerializeField, Tooltip("[Optional] If true the UI will display run the 'Error' animation trigger")]
    private bool hasErrorMenu = false;

    [Header("Details")]
    [SerializeField]
    private Text processingDetails = null;
    [SerializeField, TextArea]
    private string defaultProcessingText = null;
    [SerializeField]
    private Text errorDetails = null;
    [SerializeField, TextArea]
    private string defaultErrorText = null;

    [Header("Buttons")]
    [SerializeField]
    private Button continueButton = null;
    [SerializeField]
    private Button cancelButton = null;

    private Animator animator = null;
    private bool initialized = false;

    #region Events
    public delegate void OnCancelEvent();
    public OnCancelEvent OnCancelSelected;

    public delegate void OnContinueEvent();
    public OnContinueEvent OnContinueSelected;
    #endregion

    #region Initialization
    protected virtual void Awake() {
      Initialize();
    }

    public virtual void Initialize() {
      if (initialized == false) {
        animator = GetComponent<Animator>();
      }

      initialized = true;
    }
    #endregion

    #region Getters & Setters
    public bool HasSuccessMenu {
      get { return hasSuccessMenu; }
    }

    public bool HasErrorMenu {
      get { return hasErrorMenu; }
    }

    public bool IsClosed {
      get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("IsClosed"); }
    }

    public bool IsErrored {
      get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("InError"); }
    }

    public bool IsConnecting {
      get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("IsConnecting"); }
    }

    public Button ContinueButton {
      get { return continueButton; }
    }

    public Button CancelButton {
      get { return CancelButton; }
    }
    #endregion

    #region Display
    public void DisplayProcessing() {
      DisplayProcessing(defaultProcessingText);
    }

    public void DisplayProcessing(string aProcessingText) {
      processingDetails.text = aProcessingText;

      if (IsClosed) {
        animator.SetTrigger("Connecting");
      } else if (IsErrored) {
        animator.SetTrigger("Reconnect");
      }
    }

    public bool DisplaySuccess() {
      if (hasSuccessMenu) {
        animator.SetTrigger("Success");
        return true;
      }

      return false;
    }

    public void DisplayError(bool aShowContinueButton = false) {
      DisplayError(defaultErrorText, aShowContinueButton);
    }

    public void DisplayError(string aDetails, bool aShowContinueButton = false) {
      if (hasErrorMenu) {
        if (IsClosed || IsConnecting) {
          errorDetails.text = aDetails;

          continueButton.gameObject.SetActive(aShowContinueButton);

          animator.SetTrigger("Error");
        }
      }
    }
    #endregion

    #region Utilities
    public void SelectCancel() {
      if (OnCancelSelected != null) {
        OnCancelSelected();
      }
    }

    public void SelectContinue() {
      if (OnContinueSelected != null) {
        OnContinueSelected();
      }
    }

    public void Close(bool aForce = false) {
      if (IsClosed == false || aForce) {
        animator.SetTrigger("Close");
      }
    }

    public void Reconnect() {
      animator.SetTrigger("Reconnect");
    }
    #endregion

    #region Cleanup
    public void DestroyPopup() {
      Destroy(this.gameObject);
    }
    #endregion

  }

}
