using UnityEngine;
using System.Collections;

public class PositionGizmo : Gizmo {
    private Animator animator;

    #region Initialization
    void Awake() {
        animator = GetComponent<Animator>();
    }
    #endregion

}
