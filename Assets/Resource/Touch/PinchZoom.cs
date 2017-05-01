using UnityEngine;

namespace Resource.Touching {

    public class PinchZoom : MonoBehaviour {
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private float zoomSpeed = 0.5f;

        #region Initialization
        private void Start() {
            Initialize();
        }

        public void Initialize() {
            if (camera == null) {
                camera = Camera.main;
            }
        }
        #endregion

        #region Update
        private void Update() {
            if (Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float previousTouchMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchMangitude = (touchZero.position - touchOne.position).magnitude;

                float magnitudeDifference = previousTouchMagnitude - touchMangitude;

                // Zoom differently based on camera type
                if (camera.orthographic) {
                    camera.orthographicSize += magnitudeDifference * zoomSpeed;

                    // Make sure the orthographic size never drops below zero.
                    camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
                } else {
                    camera.fieldOfView += magnitudeDifference * zoomSpeed;

                    // Clamp the field of view to make sure it's between 0 and 180.
                    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
                }
            }
        }
        #endregion

    }

}
