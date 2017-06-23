using Extensions.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Cameras {

    public class Camera2D : MonoBehaviour {
        [SerializeField]
        private Transform mainAxis;
        [SerializeField]
        private Transform shakeAxis;
        [SerializeField]
        private new Camera camera;

        [Header("Tracking Data")]
        [SerializeField, Tooltip("Require at least one type with id \"Default\"")]
        private List<ObjectFollowData> followTypes = new List<ObjectFollowData> { new ObjectFollowData("Default", Vector3.zero, false, 1.0f) };
        [SerializeField, AxesConstraint]
        private AxesConstraint constraints;

        [Header("Shaking Data")]
        [SerializeField]
        private float defaultMagnitude;
        [SerializeField]
        private float defaultDuration;

        private Dictionary<string, ObjectFollowData> followTypesMap;
        private ObjectFollowData currentFollowType;
        private GameObject anchor;
        private Vector3 velocity;
        private Vector3 originalShakeAxisPosition;
        private Bounds bounds;

        private bool initialized = false;
        private bool isShaking = false;
        private float currentShakeTime = 0.0f;
        private float currentDuration = 0.0f;
        private float currentMagnitude = 0.0f;
        private float startingMagnitude = 0.0f;

        #region Getters & Setters
        public Camera Camera {
            get { return camera; }
        }

        public GameObject Anchor {
            get { return anchor; }
        }

        public Vector3 Offset {
            get { return currentFollowType.Offset; }
            set { currentFollowType.Offset = value; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            if (initialized == false) {
                followTypesMap = new Dictionary<string, ObjectFollowData>(followTypes.Count);
                for (int i = 0; i < followTypes.Count; i++) {
                    followTypesMap[followTypes[i].ID] = followTypes[i];
                }

                currentFollowType = followTypesMap["Default"];

                originalShakeAxisPosition = shakeAxis.localPosition;
            }

            initialized = true;
        }
        #endregion

        #region Update
        private void LateUpdate() {
            if (anchor != null) {
                Vector3 newPosition = anchor.transform.position;
                newPosition.z = mainAxis.position.z;

                newPosition += currentFollowType.Offset;

                // Bound camera position
                if (constraints.X) {
                    newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
                }

                if (constraints.Y) {
                    newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y, bounds.max.y);
                }

                if (currentFollowType.Smooth) {
                    mainAxis.position = Vector3.SmoothDamp(mainAxis.position, newPosition, ref velocity, currentFollowType.SmoothRate, float.MaxValue, Time.deltaTime);
                } else {
                    mainAxis.position = newPosition;
                }
            }

            if (isShaking) {
                currentShakeTime += Time.deltaTime;

                if (currentShakeTime < currentDuration) {
                    Vector3 offset = Random.insideUnitSphere * currentMagnitude;
                    shakeAxis.localPosition = new Vector3(offset.x, offset.y, shakeAxis.localPosition.z);

                    currentMagnitude = Mathf.Lerp(startingMagnitude, 0, currentShakeTime / currentDuration);
                } else {
                    StopShake();
                }
            }
        }
        #endregion

        #region Positioning / Anchoring
        public void SetAnchor(GameObject aAnchor, bool aInstantChange = false) {
            anchor = aAnchor;

            if (aInstantChange) {
                ResetPosition();
            }
        }

        public void ResetPosition(bool aIgnoreOffset = false) {
            if (anchor != null) {
                Vector3 newPosition = anchor.transform.position;
                newPosition.z = mainAxis.position.z;

                mainAxis.position = newPosition + ((aIgnoreOffset) ? Vector3.zero : currentFollowType.Offset);
            }
        }

        /// <summary>Set bounds of the camera (area in which it will be clamped to). By default this is border of level + camera size.</summary>
        /// <param name="aBounds">Bounds of level</param>
        public void SetBounds(Bounds aBounds) {
            bounds = aBounds;

            float verticalExtent = currentFollowType.Size;
            float horizontalExtent = verticalExtent * camera.aspect;

            bounds.min = new Vector2(bounds.min.x + horizontalExtent, bounds.min.y + verticalExtent);
            bounds.max = new Vector2(bounds.max.x - horizontalExtent, bounds.max.y - verticalExtent);
        }
        #endregion

        #region Sizing
        public void SetSize(float aSize, float aChangeRate = -1.0f) {
            if (aChangeRate >= 0) {
                StartCoroutine(ProcessSettingSize(aSize, aChangeRate));
            } else {
                camera.orthographicSize = aSize;
            }
        }

        private IEnumerator ProcessSettingSize(float aSize, float aChangeRate) {
            while (camera.orthographicSize != aSize) {
                camera.orthographicSize = Mathf.MoveTowards(camera.orthographicSize, aSize, Time.deltaTime * aChangeRate);
                yield return null;
            }
        }

        public void ResetSize() {
            camera.orthographicSize = currentFollowType.Size;
        }
        #endregion

        #region Type Management
        public void SetFollowType(string aID, bool aInstantChange = false) {
            ObjectFollowData type = null;
            if (followTypesMap.TryGetValue(aID, out type)) {
                currentFollowType = type;

                if (aInstantChange) {
                    ResetPosition();
                    ResetSize();
                } else {
                    SetSize(currentFollowType.Size, currentFollowType.ChangeSizeRate);
                }
            } else {
                Debug.LogWarning("[CameraFollow] No type of ID: " + aID + " found");
            }
        }

        public void AddType(ObjectFollowData aFollowType, bool aSetOnAddition = false) {
            followTypes.Add(aFollowType);

            if (aSetOnAddition) {
                currentFollowType = aFollowType;
            }
        }
        #endregion

        #region Shake
        public void Shake() {
            isShaking = true;

            currentShakeTime = 0.0f;
            currentDuration = defaultDuration;

            currentMagnitude = defaultMagnitude;
            startingMagnitude = currentMagnitude;
        }

        public void Shake(float aMagnitude) {
            isShaking = true;

            currentShakeTime = 0.0f;
            currentDuration = defaultDuration;

            currentMagnitude = aMagnitude;
            startingMagnitude = currentMagnitude;
        }

        public void Shake(float aMagnitude, float aDuration) {
            isShaking = true;

            currentShakeTime = 0.0f;
            currentDuration = aDuration;

            currentMagnitude = aMagnitude;
            startingMagnitude = currentMagnitude;
        }

        public void StopShake() {
            isShaking = false;

            shakeAxis.localPosition = originalShakeAxisPosition;
            currentShakeTime = 0.0f;
            currentDuration = 0.0f;

            currentMagnitude = 0.0f;
            startingMagnitude = 0.0f;
        }
        #endregion

    }

}
