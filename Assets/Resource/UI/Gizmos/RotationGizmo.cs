using UnityEngine;
using System.Collections;

public class RotationGizmo : Gizmo {
    private Animator animator;

    #region Initialization
    void Awake() {
        animator = GetComponent<Animator>();
    }
    #endregion

}
