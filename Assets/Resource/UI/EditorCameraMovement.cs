using UnityEngine;
using System.Collections;

public class EditorCameraMovement : MonoBehaviour {
    [SerializeField]
    private KeyCode acceleratorKey = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode deceleratorKey = KeyCode.LeftControl;

    [Header("Speeds")]
    [SerializeField]
    private float movementSpeed = 5.0f;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [SerializeField]
    private float zoomSpeed = 5.0f;
    [SerializeField]
    private float accelerateMultiplier = 2.0f;
    [SerializeField]
    private float decelerateMultiplier = 0.5f;

    #region Update
    void Update() {
        float multiplier = GetSpeedModifier();

        // Process Movement - middle mouse button
        if (Input.GetMouseButton(2)) {
            ProcessMovement(movementSpeed * multiplier);
        // Process Rotation - right mouse button
        } else if (Input.GetMouseButton(1)) {
            ProcessRotation(rotationSpeed * multiplier);
        // Process Zoom - scroll wheel
        } else {
            float scrollValue = Input.GetAxis("Mouse ScrollWheel");
            if (scrollValue != 0.0f) {
                ProcessZoom(scrollValue * zoomSpeed * multiplier);
            }
        }
    }
    #endregion

    #region Movement
    private void ProcessMovement(float aSpeed) {

    }
    #endregion

    #region Rotation
    private void ProcessRotation(float aSpeed) {

    }
    #endregion

    #region Zoom
    private void ProcessZoom(float aSpeed) {

    }
    #endregion

    #region Utility Functions
    private float GetSpeedModifier() {
        if (Input.GetKey(acceleratorKey)) {
            return accelerateMultiplier;
        } else if (Input.GetKey(deceleratorKey)) {
            return decelerateMultiplier;
        }

        return 1.0f;
    }
    #endregion

}
