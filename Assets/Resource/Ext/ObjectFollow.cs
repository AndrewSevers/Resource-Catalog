using System.Collections.Generic;
using UnityEngine;

namespace Resource {

    public class ObjectFollow : MonoBehaviour {
        [SerializeField, Tooltip("Require at least one type with id \"Default\"")]
        private List<ObjectFollowData> types = new List<ObjectFollowData> { new ObjectFollowData("Default", Vector3.zero, false, 1.0f) };

        private ObjectFollowData currentType;
        private Dictionary<string, ObjectFollowData> typesMap;

        private GameObject anchor;
        private Vector3 velocity;

        #region Getters & Setters
        public GameObject Anchor {
            get { return anchor; }
            set { anchor = value; }
        }

        public Vector3 Position {
            get { return transform.position; }
            set {
                Vector3 newPosition = value;
                newPosition.z = transform.position.z;

                transform.position = newPosition + currentType.Offset;
            }
        }

        public Vector3 Offset {
            get { return currentType.Offset; }
            set { currentType.Offset = value; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            typesMap = new Dictionary<string, ObjectFollowData>(types.Count);
            for (int i = 0; i < types.Count; i++) {
                typesMap[types[i].ID] = types[i];
            }

            currentType = typesMap["Default"];
        }
        #endregion

        #region Update
        private void LateUpdate() {
            if (anchor != null) {
                Vector3 newPosition = anchor.transform.position;
                newPosition.z = transform.position.z;

                if (currentType.Smooth) {
                    transform.position = Vector3.SmoothDamp(transform.position, newPosition + currentType.Offset, ref velocity, currentType.SmoothRate, float.MaxValue, Time.deltaTime);
                } else {
                    transform.position = newPosition + currentType.Offset;
                }
            }
        }
        #endregion

        #region Utility Functions
        public void ResetPosition(bool aIgnoreOffset = false) {
            if (anchor != null) {
                Vector3 newPosition = anchor.transform.position;
                newPosition.z = transform.position.z;

                transform.position = newPosition + ((aIgnoreOffset) ? Vector3.zero : currentType.Offset);
            }
        }
        #endregion

        #region Type Management
        public void SetFollowType(string aID, bool aInstantChange = false) {
            ObjectFollowData type = null;
            if (typesMap.TryGetValue(aID, out type)) {
                currentType = type;

                if (aInstantChange) {
                    ResetPosition();
                }
            } else {
                Debug.LogWarning("[CameraFollow] No type of ID: " + aID + " found");
            }
        }

        public void AddType(ObjectFollowData aFollowType, bool aSetOnAddition = false) {
            types.Add(aFollowType);

            if (aSetOnAddition) {
                currentType = aFollowType;
            }
        }
        #endregion

    }

    #region Camera Follow Data Struct
    [System.Serializable]
    public class ObjectFollowData {
        [SerializeField]
        private string id;

        [Header("Offset")]
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private bool smooth;
        [SerializeField]
        private float smoothRate;

        [Header("Sizing")]
        [SerializeField]
        private float size;
        [SerializeField]
        private float changeSizeRate;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }

        public Vector3 Offset {
            get { return offset; }
            set { offset = value; }
        }

        public bool Smooth {
            get { return smooth; }
        }

        public float SmoothRate {
            get { return smoothRate; }
        }

        public float Size {
            get { return size; }
        }

        public float ChangeSizeRate {
            get { return changeSizeRate; }
        }
        #endregion

        #region Constructor
        public ObjectFollowData(string aID, Vector3 aOffset, bool aSmooth, float aSmoothRate) {
            id = aID;
            offset = aOffset;
            smooth = aSmooth;
            smoothRate = aSmoothRate;
        }
        #endregion

    }
    #endregion

}
