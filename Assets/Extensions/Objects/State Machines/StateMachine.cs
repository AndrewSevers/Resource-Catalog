using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Extensions.States {

  [System.Serializable]
  [DisallowMultipleComponent]
  public class StateMachine : MonoBehaviour {
    [SerializeField]
    private State currentState = null;
    [SerializeField]
    private List<State> states = null;

    private State previousState = null;

    private bool changingState = false;

    #region Getters & Setters
    public bool ChangingState {
      get { return changingState; }
    }
    #endregion

    #region Initialization
    // Initialize all states. Enable and start executing the current state (chosen in the inspector).
    void Start() {
      Initialize();
    }

    private void Initialize() {
      if (states != null) {
        foreach (State state in states) {
          if (state != null) {
            state.Initialize(this);
          }
        }

        if (currentState != null) {
          currentState.Enable();
        } else {
          Debug.LogError(string.Format("StatMachine: {0} does not have a current state. Please set it in the editor!", this));
        }
      }
    }
    #endregion

    #region Update Functions
    private void Update() {
      if (currentState != null) {
        currentState.Action();
      } else {
        Debug.LogError(string.Format("StatMachine: {0} does not have a current state. Please set it in the editor!", this));
      }
    }
    #endregion

    #region State Transitions
    /// <summary>
    /// Change the state given a valid state key. Store the previous state in case we want to go back.
    /// </summary>
    public void ChangeState(string aStateKey) {
      ChangeState(aStateKey, null);
    }

    /// <summary>
    /// Change the state given a valid state key. Store the previous state in case we want to go back.
    /// </summary>
    public void ChangeState(string aStateKey, Dictionary<string, object> aData) {
      changingState = true;

      if (states != null && string.IsNullOrEmpty(aStateKey) == false) {
        State nextState = states.Find(s => s.Key == aStateKey.Trim());
        if (nextState != null && currentState.CanTransition(nextState)) {
          currentState.Disable();
          previousState = currentState;
          currentState = nextState;
          currentState.Enable(aData);
        }

#if UNITY_EDITOR
        if (nextState == null) {
          Debug.LogWarning(string.Format("State: {0} on Object: {1} does not exist in StateMachine", aStateKey, gameObject));
        }
#endif
      }

      changingState = false;
    }

    public void ReturnToPreviousState() {
      if (previousState != null && changingState == false) {
        changingState = true;

        currentState.Disable();
        currentState = previousState;
        currentState.Enable();

        previousState = null;
        changingState = false;
      }
    }
    #endregion

  }
}
