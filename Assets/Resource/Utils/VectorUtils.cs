using UnityEngine;
using System.Collections;

namespace Resource.Utils {

	public static class VectorUtils {

		#region Vector2 
		public static bool WithinRange(Vector2 a, Vector2 b, float aMaxDistance) {
			return Vector2.SqrMagnitude(a - b) <= (aMaxDistance * aMaxDistance);
		}

		public static bool WithinRange(Vector2 aOrigin, Vector2 aDestination, float xMax, float yMax) {
			Vector2 point = (aDestination - aOrigin);
			return (Mathf.Abs(point.x) <= xMax && Mathf.Abs(point.y) <= yMax);
		}
		#endregion

		#region Vector3
		public static bool WithinRange(Vector3 a, Vector3 b, float aMaxDistance) {
			return Vector3.SqrMagnitude(a - b) < (aMaxDistance * aMaxDistance);
		}

		public static bool WithinRange(Vector3 aOrigin, Vector3 aDestination, float xMax, float yMax) {
			Vector2 point = (aDestination - aOrigin);
			return (Mathf.Abs(point.x) <= xMax && Mathf.Abs(point.y) <= yMax);
		}
		#endregion

	}

}
