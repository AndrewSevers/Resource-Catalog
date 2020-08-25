using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Utils {

	public static class CameraUtils {
		public static Bounds OrthographicBounds(this Camera camera) {
			float screenAspect = (float) Screen.width / (float) Screen.height;
			float cameraHeight = camera.orthographicSize * 2;
			return new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0)); ;
		}

		public static Bounds OrthographicProjectionBounds(this Camera camera) {
			float distance = Vector3.Distance(camera.cameraToWorldMatrix.GetColumn(3), camera.cameraToWorldMatrix.GetColumn(0));
			float cameraHeight = camera.orthographicSize * 2;
			return new Bounds(camera.transform.position, new Vector3(cameraHeight * distance, cameraHeight, 0)); ;
		}
	}

}
