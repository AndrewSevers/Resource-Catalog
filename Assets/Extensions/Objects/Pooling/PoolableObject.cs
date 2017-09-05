using Extensions.Properties;
using System.Collections;
using UnityEngine;

namespace Extensions.Pooling {

    public class PoolableObject : MonoBehaviour {
        [SerializeField]
        private string id;
        [SerializeField, Min(1)]
        private int initialPoolSize = 1;

		private Pool associatedPool;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }

        public int InitialPoolSize {
            get { return initialPoolSize; }
        }
        #endregion

        #region Initialization
        public virtual void Initialize(Pool aAssociatedPool) {
			associatedPool = aAssociatedPool;
		}
		#endregion

		#region OnFree
        /// <summary>When the object is freed from the pool</summary>
		public virtual void OnFree() {
			gameObject.SetActive(true);
		}

        /// <summary>When the object is freed from the pool</summary>
        public virtual void OnFree(float aTime) {
            gameObject.SetActive(true);
            StartTimedRepool(aTime);
        }
        #endregion

        #region Cleanup
        /// <summary>Force the object to repool after a certain amount of time</summary>
        /// <param name="aTime">Time until forced repooling</param>
        public virtual void StartTimedRepool(float aTime) {
            StartCoroutine(ProcessTimedRepool(aTime));
        }

        protected virtual IEnumerator ProcessTimedRepool(float aTime) {
            yield return new WaitForSeconds(aTime);
            Repool();
        }

        /// <summary>Repool the object</summary>
		public virtual void Repool() {
            StopAllCoroutines();

			if (associatedPool != null) {
				associatedPool.Return(this);
			} else {
				Destroy(gameObject);
			}
		}
		#endregion

	}

}
