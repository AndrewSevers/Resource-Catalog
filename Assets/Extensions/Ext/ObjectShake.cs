using System.Collections;
using UnityEngine;

namespace Extensions {

    /// <summary>
    /// Shake an object based on its local position (without effecting any parent object's transform)
    /// </summary>
    public class ObjectShake : MonoBehaviour {
        [SerializeField]
        private float defaultMagnitude = 0.25f;
        [SerializeField]
        private float defaultDuration = 0.25f;

        private Coroutine shakingCoroutine;
        private Vector3 originalPosition;

        #region Getters & Setters
        public bool IsShaking {
            get { return (shakingCoroutine != null); }
        }

        public float DefaultMagnitude {
            get { return defaultMagnitude; }
        }

        public float DefaultDuration {
            get { return defaultDuration; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            originalPosition = transform.position;
        }
        #endregion

        #region Shake
        public void Shake() {
            if (shakingCoroutine != null) {
                StopCoroutine(shakingCoroutine);
            }

            shakingCoroutine = StartCoroutine(ProcessShake(defaultMagnitude, defaultDuration));
        }

        public void Shake(float aMagnitude) {
            if (shakingCoroutine != null) {
                StopCoroutine(shakingCoroutine);
            }

            shakingCoroutine = StartCoroutine(ProcessShake(aMagnitude, defaultDuration));
        }

        public void Shake(float aMagnitude, float aDuration) {
            if (shakingCoroutine != null) {
                StopCoroutine(shakingCoroutine);
            }

            shakingCoroutine = StartCoroutine(ProcessShake(aMagnitude, aDuration));
        }

        private IEnumerator ProcessShake(float aMagnitude, float aDuration = 1.0f) {
            float time = 0.0f;
            float startingMagnitude = aMagnitude;

            while (time < aDuration) {
                time += Time.deltaTime;

                Vector3 offset = Random.insideUnitSphere * aMagnitude;
                transform.localPosition = new Vector3(offset.x, offset.y, transform.localPosition.z);

                aMagnitude = Mathf.Lerp(startingMagnitude, 0, time / aDuration);

                yield return null;
            }

            transform.localPosition = originalPosition;

            shakingCoroutine = null;
        }
        #endregion

        #region Cleanup 
        public void Stop() {
            if (shakingCoroutine != null) {
                StopCoroutine(shakingCoroutine);
                shakingCoroutine = null;
            }
        }
        #endregion

    }

    #region Object Shake Data Class
    [System.Serializable]
    public struct ObjectShakeData {
        [SerializeField]
        private float magnitude;
        [SerializeField]
        private float duration;

        #region Getters & Setters
        public float Magnitude {
            get { return magnitude; }
        }

        public float Duration {
            get { return duration; }
        }
        #endregion

        #region Constructor
        public ObjectShakeData(float aMagnitude, float aDuration) {
            magnitude = aMagnitude;
            duration = aDuration;
        }
        #endregion
    }
    #endregion

}
