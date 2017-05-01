using UnityEngine;
using System.Collections;

namespace Resource.Utils {

	public static class DebugUtils {

		#region Arrows
		public static void DrawArrow(bool aForGizmo, Vector3 aOrigin, Vector3 aDirection, float aDuration = 1.0f, float aArrowLength = 0.25f, float aArrowAngle = 20.0f) {
			if (aForGizmo) {
				Gizmos.DrawRay(aOrigin, aDirection);
			} else {
				Debug.DrawRay(aOrigin, aDirection, Gizmos.color, aDuration);
			}

			DrawArrowEnd(aForGizmo, aOrigin, aDirection, Gizmos.color, aDuration, aArrowLength, aArrowAngle);
		}

		public static void DrawArrow(bool aForGizmo, Vector3 aOrigin, Vector3 aDirection, Color aColor, float aDuration = 1.0f, float aArrowLength = 0.25f, float aArrowAngle = 20.0f) {
			if (aForGizmo) {
				Gizmos.color = aColor;
				Gizmos.DrawRay(aOrigin, aDirection);
			} else {
				Debug.DrawRay(aOrigin, aDirection, aColor, aDuration);
			}

			DrawArrowEnd(aForGizmo, aOrigin, aDirection, aColor, aDuration, aArrowLength, aArrowAngle);
		}

		private static void DrawArrowEnd(bool aForGizmo, Vector3 aOrigin, Vector3 aDirection, Color aColor, float aDuration = 1.0f, float aArrowLength = 0.25f, float aArrowAngle = 20.0f) {
			Vector3 right = Quaternion.LookRotation(aDirection) * Quaternion.Euler(aArrowAngle, 0, 0) * Vector3.back;
			Vector3 left = Quaternion.LookRotation(aDirection) * Quaternion.Euler(-aArrowAngle, 0, 0) * Vector3.back;
			Vector3 up = Quaternion.LookRotation(aDirection) * Quaternion.Euler(0, aArrowAngle, 0) * Vector3.back;
			Vector3 down = Quaternion.LookRotation(aDirection) * Quaternion.Euler(0, -aArrowAngle, 0) * Vector3.back;
			if (aForGizmo) {
				Gizmos.color = aColor;
				Gizmos.DrawRay(aOrigin + aDirection, right * aArrowLength);
				Gizmos.DrawRay(aOrigin + aDirection, left * aArrowLength);
				Gizmos.DrawRay(aOrigin + aDirection, up * aArrowLength);
				Gizmos.DrawRay(aOrigin + aDirection, down * aArrowLength);
			} else {
				Debug.DrawRay(aOrigin + aDirection, right * aArrowLength, aColor, aDuration);
				Debug.DrawRay(aOrigin + aDirection, left * aArrowLength, aColor, aDuration);
				Debug.DrawRay(aOrigin + aDirection, up * aArrowLength, aColor, aDuration);
				Debug.DrawRay(aOrigin + aDirection, down * aArrowLength, aColor, aDuration);
			}
		}
		#endregion

		#region Boxes
		//Draws just the box at where it is currently hitting.
		public static void DrawBoxCastOnHit(Vector3 aOrigin, Vector3 aHalfExtents, Quaternion aOrientation, Vector3 aDirection, float aHitInfoDistance, Color aColor) {
			aOrigin = CastCenterOnCollision(aOrigin, aDirection, aHitInfoDistance);
			DrawBox(aOrigin, aHalfExtents, aOrientation, aColor);
		}

		//Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
		public static void DrawBoxCastBox(Vector3 aOrigin, Vector3 aHalfExtents, Quaternion aOrientation, Vector3 aDirection, float aDistance, Color aColor, float aDuration = 1.0f) {
			aDirection.Normalize();
			Box bottomBox = new Box(aOrigin, aHalfExtents, aOrientation);
			Box topBox = new Box(aOrigin + (aDirection * aDistance), aHalfExtents, aOrientation);

			Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, aColor, aDuration);
			Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, aColor, aDuration);
			Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, aColor, aDuration);
			Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, aColor, aDuration);
			Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, aColor, aDuration);
			Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, aColor, aDuration);
			Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, aColor, aDuration);
			Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, aColor, aDuration);

			DrawBox(bottomBox, aColor);
			DrawBox(topBox, aColor);
		}

		public static void DrawBox(Vector3 aOrigin, Vector3 aHalfExtents, Quaternion aOrientation, Color color) {
			DrawBox(new Box(aOrigin, aHalfExtents, aOrientation), color);
		}

		public static void DrawBox(Box aBox, Color aColor, float aDuration = 1.0f) {
			Debug.DrawLine(aBox.frontTopLeft, aBox.frontTopRight, aColor, aDuration);
			Debug.DrawLine(aBox.frontTopRight, aBox.frontBottomRight, aColor, aDuration);
			Debug.DrawLine(aBox.frontBottomRight, aBox.frontBottomLeft, aColor, aDuration);
			Debug.DrawLine(aBox.frontBottomLeft, aBox.frontTopLeft, aColor, aDuration);

			Debug.DrawLine(aBox.backTopLeft, aBox.backTopRight, aColor, aDuration);
			Debug.DrawLine(aBox.backTopRight, aBox.backBottomRight, aColor, aDuration);
			Debug.DrawLine(aBox.backBottomRight, aBox.backBottomLeft, aColor, aDuration);
			Debug.DrawLine(aBox.backBottomLeft, aBox.backTopLeft, aColor, aDuration);

			Debug.DrawLine(aBox.frontTopLeft, aBox.backTopLeft, aColor, aDuration);
			Debug.DrawLine(aBox.frontTopRight, aBox.backTopRight, aColor, aDuration);
			Debug.DrawLine(aBox.frontBottomRight, aBox.backBottomRight, aColor, aDuration);
			Debug.DrawLine(aBox.frontBottomLeft, aBox.backBottomLeft, aColor, aDuration);
		}

		#region Box Struct
		public struct Box {
			public Vector3 localFrontTopLeft { get; private set; }
			public Vector3 localFrontTopRight { get; private set; }
			public Vector3 localFrontBottomLeft { get; private set; }
			public Vector3 localFrontBottomRight { get; private set; }
			public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
			public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
			public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
			public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

			public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
			public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
			public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
			public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
			public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
			public Vector3 backTopRight { get { return localBackTopRight + origin; } }
			public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
			public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

			public Vector3 origin { get; private set; }

			public Box(Vector3 aOrigin, Vector3 aHalfExtents) {
				this.localFrontTopLeft = new Vector3(-aHalfExtents.x, aHalfExtents.y, -aHalfExtents.z);
				this.localFrontTopRight = new Vector3(aHalfExtents.x, aHalfExtents.y, -aHalfExtents.z);
				this.localFrontBottomLeft = new Vector3(-aHalfExtents.x, -aHalfExtents.y, -aHalfExtents.z);
				this.localFrontBottomRight = new Vector3(aHalfExtents.x, -aHalfExtents.y, -aHalfExtents.z);

				this.origin = aOrigin;
			}

			public Box(Vector3 aOrigin, Vector3 aHalfExtents, Quaternion aOrientation) : this(aOrigin, aHalfExtents) {
				Rotate(aOrientation);
			}

			public void Rotate(Quaternion aOrientation) {
				localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, aOrientation);
				localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, aOrientation);
				localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, aOrientation);
				localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, aOrientation);
			}
		}
		#endregion

		private static Vector3 CastCenterOnCollision(Vector3 aOrigin, Vector3 aDirection, float aHitInfoDistance) {
			return aOrigin + (aDirection.normalized * aHitInfoDistance);
		}

		private static Vector3 RotatePointAroundPivot(Vector3 aPoint, Vector3 aPivot, Quaternion aRotation) {
			Vector3 direction = aPoint - aPivot;
			return aPivot + aRotation * direction;
		}
		#endregion

	}

}
