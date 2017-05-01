using UnityEngine;

namespace Resource.Cameras {

    /// <summary>
    /// Alter the camera's projection matrix (for orthographic cameras) so off aspect ratios match the provided aspect
    /// </summary>
    [RequireComponent(typeof(Camera))]
	public class CameraOrthographicProjection : MonoBehaviour {
		[SerializeField]
		private float orthographicSize = 5;
		[SerializeField]
		private float aspect;

		#region Intiailization
		void Awake() {
			SetProjectionMatrix(orthographicSize, aspect);
		}
		#endregion

		#region Matrix Manipulation
		public void SetProjectionMatrix(float aOrthographicSize, float aAspect) {
			Camera camera = GetComponent<Camera>();
			camera.projectionMatrix = Matrix4x4.Ortho(-aOrthographicSize * aAspect, aOrthographicSize * aAspect, -aOrthographicSize, aOrthographicSize, camera.nearClipPlane, camera.farClipPlane);
		}

		public void ResetMatrix() {
			Camera camera = GetComponent<Camera>();
			camera.ResetProjectionMatrix();
		}
		#endregion

	}

}


