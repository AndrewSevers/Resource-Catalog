using UnityEngine;
using System.Collections.Generic;

namespace Extensions.States {

  [System.Serializable]
  public class State : MonoBehaviour {
    [SerializeField]
    private string key = null;
    [SerializeField]
    private List<State> transitions = null;

    private StateMachine stateMachine = null;

    // safeToDelete is used only in a custom inspector so it will always give a warning about not being used
#pragma warning disable 0414
    private bool safeToDelete = false;
#pragma warning restore 0414

    #region Getters & Setters
    public string Key {
      get { return key; }
    }

    protected StateMachine StateMachine {
      get { return stateMachine; }
    }
    #endregion

    #region Abstract Functions
    public virtual void Initialize(StateMachine aStateMachine) {
      stateMachine = aStateMachine;
    }

    public virtual void Enable(Dictionary<string, object> aData = null) { }
    public virtual void Action() { }
    public virtual void Disable() { }
    #endregion

    #region Transition Functions
    public bool CanTransition(State aState) {
      if (transitions != null) {
        if (transitions.Contains(aState)) {
          return true;
        }
      }

#if UNITY_EDITOR
      Debug.LogWarning(string.Format("Transition \"{0}\" on object \"{1}\" does not exist", aState.GetType().Name, gameObject.name));
#endif

      return false;
    }
    #endregion

  }

}
