using Extensions.Properties;
using System.Collections;
using UnityEngine;

namespace Extensions.Utils {

    public class SplineTraveler : MonoBehaviour {
		[SerializeField]
		public BezierSpline spline;

        [Header("Movement")]
		[SerializeField]
		private float duration;
        [SerializeField, Range(0, 1)]
        private float startingTime = 0.0f;
        [SerializeField]
        private TravelDirection movementDirection = TravelDirection.Forwards;
        [SerializeField]
        private FacingDirection facingDirection = FacingDirection.Right;
        [SerializeField]
		private TraversalMode mode;
        [SerializeField]
        private bool ease;
        [SerializeField, Visibility("ease", true)]
        private AnimationCurve easingCurve;

        [Header("Rotation")]
        [SerializeField]
        private bool faceForward = false;
        [SerializeField, Visibility("faceForward", true)]
        private bool rotationIs2D = false;

        [SerializeField, ReadOnly]
        private bool active = true;
        [SerializeField, ReadOnly]
		private float progress;
        private new Rigidbody2D rigidbody2D;
        private new Rigidbody rigidbody;

        #region Spline Section Class
        [System.Serializable]
        public class SplineSection {
            [SerializeField]
            private float rate;

            #region Getters & Setters
            public float Rate {
                get { return rate; }
            }
            #endregion
        }
        #endregion

        #region Getters & Setters
        public bool Active {
            get { return active; }
            set { active = value; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody = GetComponent<Rigidbody>();

            if (progress == 0) {
                progress = startingTime;
            }

            if (facingDirection == FacingDirection.Left) {
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            }
        }
        #endregion

        #region Activation
        public void Activate(BezierSpline aSpline) {
            Activate(aSpline, startingTime, duration);
        }

        public void Activate(BezierSpline aSpline, float aStartingTime) {
            Activate(aSpline, aStartingTime, duration);
        }

        public void Activate(BezierSpline aSpline, float aStartingTime, float aDuration) {
            spline = aSpline;            
            progress = aStartingTime;
            duration = aDuration;

            active = true;
        }
        #endregion

        #region Update
        private void Update() {
            if (active == false || movementDirection == TravelDirection.None) {
                return;
            }

            if (movementDirection == TravelDirection.Forwards) {
                progress += Time.deltaTime / duration;

                if (progress > 1f) {
                    switch (mode) {
                        case TraversalMode.Once:
                            progress = 1.0f;
                            break;
                        case TraversalMode.Loop:
                            StartCoroutine(MoveToStart(movementDirection));
                            break;
                        case TraversalMode.QuickLoop:
                            progress = 0.0f;
                            break;
                        case TraversalMode.PingPong:
                            progress = 1.0f;
                            movementDirection = TravelDirection.Backwards;
                            break;
                        case TraversalMode.Extend:
                            progress = 0.0f;

                            // Re-orient spline
                            spline.MoveTo(transform.localPosition, BezierEdgeType.Start);
                            break;
                    }
                }
            } else if (movementDirection == TravelDirection.Backwards) {
                progress -= Time.deltaTime / duration;

                if (progress < 0f) {
                    switch (mode) {
                        case TraversalMode.Once:
                            progress = 0.0f;
                            break;
                        case TraversalMode.Loop:
                            StartCoroutine(MoveToStart(movementDirection));
                            break;
                        case TraversalMode.QuickLoop:
                            progress = 1.0f;
                            break;
                        case TraversalMode.PingPong:
                            progress = 0.0f;
                            movementDirection = TravelDirection.Forwards;
                            break;
                        case TraversalMode.Extend:
                            progress = 1.0f;

                            // Re-orient spline
                            spline.MoveTo(transform.localPosition, BezierEdgeType.End);
                            break;

                    }
                }
            }
            
			Vector3 position = (ease) ? spline.GetPoint(easingCurve.Evaluate(progress)) : spline.GetPoint(progress);

            if (rigidbody2D != null) {
                rigidbody2D.MovePosition(position);
            } else if (rigidbody != null) {
                rigidbody.MovePosition(position);
            } else {
                transform.position = position;
            }

            if (faceForward) {
                if (rotationIs2D) {
                    Vector3 facingDirection = Vector3.zero;
                    if (movementDirection == TravelDirection.Forwards) {
                        float futureProgress = progress + Time.deltaTime + 0.1f;
                        facingDirection = (futureProgress >= 1) ? spline.GetPoint(Time.deltaTime) - spline.GetPoint(0) : spline.GetPoint(futureProgress) - position;
                    } else {
                        float futureProgress = progress - Time.deltaTime - 0.1f;
                        facingDirection = (futureProgress <= 0) ? spline.GetPoint(1 - Time.deltaTime) - spline.GetPoint(1) : spline.GetPoint(futureProgress) - position;
                    }

                    transform.right = facingDirection;
                } else {
                    transform.LookAt(position + spline.GetDirection(progress));
                }
            }
		}
        #endregion

        #region Utilities
        private IEnumerator MoveToStart(TravelDirection aOriginalDirection) {
            Vector3 endPoint = Vector3.zero;
            switch (movementDirection) {
                case TravelDirection.Forwards:
                    endPoint = spline.GetPoint(0.0f);
                    break;
                case TravelDirection.Backwards:
                    endPoint = spline.GetPoint(1.0f);
                    break;
            }

            do {
                Vector3 position = Vector3.MoveTowards(transform.position, endPoint, Time.deltaTime);

                if (rigidbody2D != null) {
                    rigidbody2D.MovePosition(position);
                } else if (rigidbody != null) {
                    rigidbody.MovePosition(position);
                } else {
                    transform.position = position;
                }

                yield return null;
            } while (endPoint != transform.position);
        }
        #endregion

    }

}
