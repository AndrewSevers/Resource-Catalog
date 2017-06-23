using UnityEngine;

namespace Extensions.Utils {

	public static class Bezier {

		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float aTime) {
			aTime = Mathf.Clamp01(aTime);
			float oneMinusTime = 1f - aTime;
			return
				oneMinusTime * oneMinusTime * p0 +
				2f * oneMinusTime * aTime * p1 +
				aTime * aTime * p2;
		}

		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float aTime) {
			return
				2f * (1f - aTime) * (p1 - p0) +
				2f * aTime * (p2 - p1);
		}

		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float aTime) {
			aTime = Mathf.Clamp01(aTime);
			float oneMinusTime = 1f - aTime;
			return
				oneMinusTime * oneMinusTime * oneMinusTime * p0 +
				3f * oneMinusTime * oneMinusTime * aTime * p1 +
				3f * oneMinusTime * aTime * aTime * p2 +
				aTime * aTime * aTime * p3;
		}

		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float aTime) {
			aTime = Mathf.Clamp01(aTime);
			float oneMinusTime = 1f - aTime;
			return
				3f * oneMinusTime * oneMinusTime * (p1 - p0) +
				6f * oneMinusTime * aTime * (p2 - p1) +
				3f * aTime * aTime * (p3 - p2);
		}
	}

}
