using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Utils {

	public static class CameraUtils {
		public static Bounds OrthographicBounds(this UnityEngine.Camera aCamera) {
			float screenAspect = (float) Screen.width / (float) Screen.height;
			float cameraHeight = aCamera.orthographicSize * 2;
			return new Bounds(aCamera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0)); ;
		}

		public static Bounds OrthographicProjectionBounds(this UnityEngine.Camera aCamera) {
			float distance = Vector3.Distance(aCamera.cameraToWorldMatrix.GetColumn(3), aCamera.cameraToWorldMatrix.GetColumn(0));
			//Debug.Log(Vector3.Distance(aCamera.cameraToWorldMatrix.GetColumn(3), aCamera.cameraToWorldMatrix.GetColumn(0)));
			//float screenAspect = (float) Screen.width / (float) Screen.height;
			float cameraHeight = aCamera.orthographicSize * 2;
			return new Bounds(aCamera.transform.position, new Vector3(cameraHeight * distance, cameraHeight, 0)); ;
		}
	}

}
