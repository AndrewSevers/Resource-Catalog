using UnityEngine;

namespace Extensions.Utils {

    [System.Serializable]
    public class BezierPoint {
        [SerializeField]
        private Vector3 origin;
        [SerializeField]
        private Vector3 startTangent = new Vector3(-1.0f, 0, 0);
        [SerializeField]
        private Vector3 endTangent = new Vector3(1.0f, 0, 0);
        [SerializeField]
        private BezierControlPointMode mode;

        #region Getters & Setters
        public Vector3 Origin {
            get { return origin; }
            set { origin = value; }
        }

        public Vector3 StartTangent {
            get { return startTangent; }
            set { startTangent = value; }
        }

        public Vector3 EndTangent {
            get { return endTangent; }
            set { endTangent = value; }
        }

        public BezierControlPointMode Mode {
            get { return mode; }
            set { mode = value; }
        }
        #endregion

        #region Constructors
        public BezierPoint(Vector3 aPosition) {
            origin = aPosition;
        }

        public BezierPoint(Vector3 aPosition, BezierControlPointMode aMode) {
            origin = aPosition;
            mode = aMode;
        }
        #endregion

        #region Utility Functions
        public Vector3 GetPoint(BezierPointType aType) {
            switch (aType) {
                case BezierPointType.Origin:
                    return origin;
                case BezierPointType.StartTangent:
                    return startTangent;
                case BezierPointType.EndTangent:
                    return endTangent;
                default:
                    Debug.LogWarning("[BezierPoint] Invalid BezierPointType: " + aType);
                    return Vector3.zero;
            }
        }
        #endregion

    }

}
