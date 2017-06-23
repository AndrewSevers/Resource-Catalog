using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {
    [SerializeField]
    private GizmoType type;

    protected GameObject attachedObject;

    #region Getters & Setters
    public GizmoType Type {
        get { return type; }
    }
    #endregion

}

public enum GizmoType {
    Position = 1,
    Rotation = 2,
    Scale = 3
}
