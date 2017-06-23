using System;
using UnityEngine;

namespace Extensions.Utils {

    public class BezierSpline : MonoBehaviour {
		[SerializeField]
		private Vector3[] points;
		[SerializeField]
		private BezierControlPointMode[] modes;
		[SerializeField]
		private bool loop;

		#region Getters & Setters
		public Vector3[] Points {
			get { return points; }
		}

		public BezierControlPointMode[] Modes {
			get { return modes; }
		}

		public bool Loop {
			get { return loop; }
			set {
				loop = value;
				if (value == true) {
					modes[modes.Length - 1] = modes[0];
					SetControlPoint(0, points[0]);
				}
			}
		}

		public int ControlPointCount {
			get { return points.Length; }
		}

		public int CurveCount {
			get { return (points.Length - 1) / 3; }
		}
		#endregion

		#region Control Points
		public Vector3 GetControlPoint(int aIndex) {
			return (aIndex < points.Length) ? points[aIndex] : default(Vector3);
		}

		public void SetControlPoint(int aIndex, Vector3 aPoint) {
			if (aIndex % 3 == 0) {
				Vector3 delta = aPoint - points[aIndex];
				if (loop) {
					if (aIndex == 0) {
						points[1] += delta;
						points[points.Length - 2] += delta;
						points[points.Length - 1] = aPoint;
					} else if (aIndex == points.Length - 1) {
						points[0] = aPoint;
						points[1] += delta;
						points[aIndex - 1] += delta;
					} else {
						points[aIndex - 1] += delta;
						points[aIndex + 1] += delta;
					}
				} else {
					if (aIndex > 0) {
						points[aIndex - 1] += delta;
					}
					if (aIndex + 1 < points.Length) {
						points[aIndex + 1] += delta;
					}
				}
			}
			points[aIndex] = aPoint;
			EnforceMode(aIndex);
		}
		#endregion

		#region Control Point Mode
		public BezierControlPointMode GetControlPointMode(int aIndex) {
			return (aIndex < ((aIndex + 1) / 3)) ? modes[(aIndex + 1) / 3] : BezierControlPointMode.Free;
		}

		public void SetControlPointMode(int aIndex, BezierControlPointMode aMode) {
			int modeIndex = (aIndex + 1) / 3;
			modes[modeIndex] = aMode;
			if (loop) {
				if (modeIndex == 0) {
					modes[modes.Length - 1] = aMode;
				} else if (modeIndex == modes.Length - 1) {
					modes[0] = aMode;
				}
			}
			EnforceMode(aIndex);
		}
		#endregion

		#region Curve 
		/// <summary>
		/// Add another point to the curve
		/// </summary>
		public void AddCurve() {
			Vector3 point = points[points.Length - 1];
			Array.Resize(ref points, points.Length + 3);
			point.x += 1f;
			points[points.Length - 3] = point;
			point.x += 1f;
			points[points.Length - 2] = point;
			point.x += 1f;
			points[points.Length - 1] = point;

			Array.Resize(ref modes, modes.Length + 1);
			modes[modes.Length - 1] = modes[modes.Length - 2];
			EnforceMode(points.Length - 4);

			if (loop) {
				points[points.Length - 1] = points[0];
				modes[modes.Length - 1] = modes[0];
				EnforceMode(0);
			}
		}

		public void ConnectCurve(BezierSpline aOtherSpline) {
			if (aOtherSpline.Loop == false) {
				ConnectCurve(aOtherSpline.Points, aOtherSpline.Modes);
			} else {
				Debug.LogWarning("Invalid curve connection! Can't connect to a curve that loop");
			}
		}

		public void ConnectCurve(Vector3[] aOtherPoints, BezierControlPointMode[] aOtherModes) {
			if (loop == false) {
				Vector3 point = points[points.Length - 1];
				int pointIndex = points.Length;

				Array.Resize(ref points, points.Length + aOtherPoints.Length + 1);

				// Setup new point for connection
				points[pointIndex++] = point;

				// Combine points from other curve
				for (int i = 0; i < aOtherPoints.Length; i++) {
					points[pointIndex++] = aOtherPoints[i];
				}

				// Combine modes from other curve
				int modeIndex = modes.Length;
				Array.Resize(ref modes, modes.Length + aOtherModes.Length);

				for (int i = 0; i < aOtherModes.Length; i++) {
					modes[modeIndex++] = aOtherModes[i];
				}
			} else {
				Debug.LogWarning("Invalid curve connection! Looping curves can't connect to other curves");
			}
		}
        #endregion

        #region Points
        public void RemovePoint(int aIndex) {
            Array.Resize(ref points, points.Length - 3);
            Array.Resize(ref modes, modes.Length - 1);
        }
        #endregion

        #region Point Mode
        private void EnforceMode(int aIndex) {
			int modeIndex = (aIndex + 1) / 3;
			BezierControlPointMode mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || loop == false && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
				return;
			}

			int middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (aIndex <= middleIndex) {
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0) {
					fixedIndex = points.Length - 2;
				}
				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= points.Length) {
					enforcedIndex = 1;
				}
			} else {
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= points.Length) {
					fixedIndex = 1;
				}
				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0) {
					enforcedIndex = points.Length - 2;
				}
			}

			Vector3 middle = points[middleIndex];
			Vector3 enforcedTangent = middle - points[fixedIndex];
			if (mode == BezierControlPointMode.Aligned) {
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
			}
			points[enforcedIndex] = middle + enforcedTangent;
		}
		#endregion

		#region Utility Functions
		/// <summary>
		/// Get a point on the curve according to the given timeline along it
		/// </summary>
		/// <param name="aTime">Time along the curve</param>
		public Vector3 GetPoint(float aTime) {
			int i;
			if (aTime >= 1f) {
				aTime = 1f;
				i = points.Length - 4;
			} else {
				aTime = Mathf.Clamp01(aTime) * CurveCount;
				i = (int) aTime;
				aTime -= i;
				i *= 3;
			}

			return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], aTime));
		}

		/// <summary>
		/// Get the current velocity and the given timeline along the curve
		/// </summary>
		/// <param name="aTime">Time along the curve</param>
		public Vector3 GetVelocity(float aTime) {
			int i;
			if (aTime >= 1f) {
				aTime = 1f;
				i = points.Length - 4;
			} else {
				aTime = Mathf.Clamp01(aTime) * CurveCount;
				i = (int) aTime;
				aTime -= i;
				i *= 3;
			}
			return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], aTime)) - transform.position;
		}

		/// <summary>
		/// Get the facing direction along the curve at the given time
		/// </summary>
		/// <param name="aTime">Time along the curve</param>
		public Vector3 GetDirection(float aTime) {
			return GetVelocity(aTime).normalized;
		}

        /// <summary>
        /// Move the entire spline (all of its points) to the desired position
        /// </summary>
        /// <param name="aPosition">Position to move the spline to</param>
        public void MoveTo(Vector3 aPosition, BezierEdgeType aEdgeType) {
            Vector3 offset = (aEdgeType == BezierEdgeType.Start) ? aPosition - points[0] : aPosition - points[points.Length -1];

            for (int i = 0; i < points.Length; i++) {
                points[i] += offset;
            }
        }
        #endregion

        #region Editor Functions
        [ContextMenu("Flip Horizontally")]
        public void FlipHorizontally() {
            for (int i = 0; i < points.Length; i++) {
                points[i].x = -points[i].x;
            }
        }

        [ContextMenu("Flip Vertically")]
        public void FlipVertically() {
            for (int i = 0; i < points.Length; i++) {
                points[i].y = -points[i].y;
            }
        }
        #endregion

        #region Cleanup
        public void Reset() {
			points = new Vector3[] {
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f)
		};
			modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
		}
		#endregion

	}

}
