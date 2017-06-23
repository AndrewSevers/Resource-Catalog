using UnityEngine;
using System.Collections;

public class ScaleGizmo : Gizmo {
    private Animator animator;

    #region Initialization
    void Awake() {
        animator = GetComponent<Animator>();
    }
    #endregion

}
