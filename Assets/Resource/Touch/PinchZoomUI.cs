using UnityEngine;

namespace Resource.Touching {

    public class PinchZoomUI : MonoBehaviour {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private float zoomSpeed = 0.5f;

        #region Update
        private void Update() {
#if UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            switch (TouchSimulator.TouchCount) {
                case 2:
                    Zoom(TouchSimulator.Touches[0], TouchSimulator.Touches[1]);
                    break;
                case 0:
                    break;
            }
#else
            switch (Input.touchCount) {
                case 2:
                    Zoom(Input.touches[0], Input.touches[1]);
                    break;
                case 0:
                    break;
            }
#endif
        }
        #endregion

        #region Zooming
        private void Zoom(Touch aTouchOne, Touch aTouchTwo) {
            // Find the position in the previous frame of each touch.
            Vector2 touchOnePreviousPosition = aTouchOne.position - aTouchOne.deltaPosition;
            Vector2 touchTwoPreviousPosition = aTouchTwo.position - aTouchTwo.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float previousTouchMagnitude = (touchOnePreviousPosition - touchTwoPreviousPosition).magnitude;
            float touchMangitude = (aTouchOne.position - aTouchTwo.position).magnitude;

            float magnitudeDifference = previousTouchMagnitude - touchMangitude;

            canvas.scaleFactor -= magnitudeDifference * zoomSpeed;
            canvas.scaleFactor = Mathf.Clamp(canvas.scaleFactor, 0.1f, float.MaxValue);
        }
        #endregion

    }

}
