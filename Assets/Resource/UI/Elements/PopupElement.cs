using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resource.UI {

    [RequireComponent(typeof(Animator))]
    public class PopupElement : MonoBehaviour {
        #region Events
        public delegate void OnOpenEvent();
        public OnOpenEvent OnOpen;

        public delegate void OnCloseEvent();
        public OnCloseEvent OnClose;
        #endregion

        protected Animator animator;
        protected bool initialized = false;

        #region Getters & Setters
        public virtual bool Active {
            get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("Open"); }
            set {
                if (Active != value) {
                    if (gameObject.activeSelf == false) {
                        gameObject.SetActive(true);
                    }

                    if (value) {
                        Open();
                    } else {
                        Close();
                    }
                }
            }
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
        protected virtual void Open(bool aInstantly = false) {
            if (OnOpen != null) {
                OnOpen();
            }

            animator.SetTrigger("Open");
        }
        #endregion

        #region Close
        protected virtual void Close(bool aInstantly = false) {
            if (OnClose != null) {
                OnClose();
            }

            animator.SetTrigger("Close");
        }
        #endregion

            #region Utility Functions
        public virtual void Activate(bool aInstantly = false) { }
        public virtual void OnReset() { }
        #endregion

    }

}
