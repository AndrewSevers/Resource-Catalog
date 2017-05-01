using Resource.Utils;
using UnityEngine;

namespace Resource {

    /// <summary>
    /// Create a Singleton monobehaviour that does not utilize DontDestroyOnLoad
    /// To load this as a prefab when one isn't found in the scene the prefab must be stored in the 'Resources' directory of the Unity project
    /// </summary>
    public class FiniteSingleton<T> : MonoBehaviour where T : MonoBehaviour {
		protected static T instance;
		protected static object accessLock = new object();

		protected FiniteSingleton() { }

        #region Initialization
        protected virtual void Awake() {
            Initialize();
        }

        public virtual void Initialize() {
            if (instance != null && instance != this) {
                DestroyImmediate(gameObject);
            }
        }
        #endregion

        #region Getters & Setters
        public static T Instance {
			get {
				lock (accessLock) {
					if (instance == null) {
						T[] instances = FindObjectsOfType<T>();

						if (instances.Length > 0) {
							instance = instances[0];

							if (instances.Length > 1) {
								Debug.LogError(string.Format("Application contains more than one (1) instance of [Singleton] '{0}'. Reopen scene or stop creating multiple instances to correct the issue", instance));
							}
						} else {
							GameObject singleton = Resources.Load<GameObject>("Prefabs/" + typeof(T).Name);
							if (singleton == null) {
								singleton = Resources.Load<GameObject>("Prefabs/" + StringUtils.ToSpacedFormat(typeof(T).Name));
							}

							if (singleton != null) {
								singleton = Instantiate(singleton);
								instance = singleton.GetComponent<T>();

								Debug.Log(string.Format("[Singleton] instance of '{0}' was loaded from resources", typeof(T).Name));
							} else {
								singleton = new GameObject();
								instance = singleton.AddComponent<T>();

								singleton.name = string.Format("(Singleton) {0}", typeof(T).Name);

								Debug.Log(string.Format("[Singleton] instance of '{0}' was created", typeof(T).Name));
							}

							singleton.SetActive(true);
						}
					}

					return instance;
				}
			}
		}
		#endregion

	}

}
