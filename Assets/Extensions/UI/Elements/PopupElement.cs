using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.UI {

  [RequireComponent(typeof(Animator))]
  public class PopupElement : MonoBehaviour {
    public delegate void OnOpenEvent();
    public OnOpenEvent OnOpen;

    public delegate void OnCloseEvent();
    public OnCloseEvent OnClose;

    protected Animator animator;
    protected bool initialized = false;

    protected Coroutine lifetimeCoroutine = null;

    #region Getters & Setters
    public virtual bool Active {
      get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("Open"); }
    }
    #endregion

    #region Initialization
    protected virtual void Awake() {
      Initialize();
    }

    public virtual void Initialize() {
      if (initialized == false) {
        animator = GetComponent<Animator>();
      }
    }
    #endregion

    #region Open
    public virtual void Open(bool aInstantly = false) {
      if (gameObject.activeSelf == false) {
        gameObject.SetActive(true);
      }

      if (Active == false) {
        if (OnOpen != null) {
          OnOpen();
        }

        if (aInstantly) {
          animator.SetTrigger("Opened");
        } else {
          animator.SetTrigger("Open");
        }
      }
    }

    public virtual void Open(float aLifetime, bool aInstantly = false) {
      if (gameObject.activeSelf == false) {
        gameObject.SetActive(true);
      }

      if (Active == false) {
        if (OnOpen != null) {
          OnOpen();
        }

        if (aInstantly) {
          animator.SetTrigger("Opened");
        } else {
          animator.SetTrigger("Open");
        }
      }

      if (lifetimeCoroutine != null) {
        StopCoroutine(lifetimeCoroutine);
      }

      lifetimeCoroutine = StartCoroutine(ProcessActiveLifetime(aLifetime));
    }
    #endregion

    #region Close
    public virtual void Close(bool aInstantly = false) {
      if (gameObject.activeSelf == false) {
        gameObject.SetActive(true);
      }

      if (Active) {
        if (OnClose != null) {
          OnClose();
        }

        if (aInstantly) {
          animator.SetTrigger("Closed");
        } else {
          animator.SetTrigger("Close");
        }
      }
    }
    #endregion

    #region Lifetime Functions
    protected virtual IEnumerator ProcessActiveLifetime(float aLifetime) {
      yield return new WaitForSeconds(aLifetime);
      Close();
    }

    public virtual void EndLifetime() {
      if (lifetimeCoroutine != null) {
        StopCoroutine(lifetimeCoroutine);

        Close();
      }
    }
    #endregion

    #region Utility Functions
    public virtual void OnReset() { }
    #endregion

    #region Cleanup
    public virtual void DestroyPopup() {
      Destroy(gameObject);
    }
    #endregion

  }

}
