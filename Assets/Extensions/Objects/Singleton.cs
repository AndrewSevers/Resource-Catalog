using Extensions.Utils;
using UnityEngine;

namespace Extensions {

  /// <summary>
  /// Create a Singleton monobehaviour.
  /// To load this as a prefab when one isn't found in the scene the prefab must be stored in the 'Resources' directory of the Unity project
  /// </summary>
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance = null;
    private static object _lock = new object();

    private static bool _isClosing = false;

    protected Singleton() { }

    #region Initialization
    protected virtual void Awake() {
      Initialize();
    }

    public virtual void Initialize() {
      if (_instance != null && _instance != this) {
        DestroyImmediate(gameObject);
      }
    }
    #endregion

    #region Getters & Setters
    public static T Instance {
      get {
        if (_isClosing) {
          Debug.LogWarning("Application is closing [Singleton] had been destroyed and is no longer accessible");
          return null;
        }

        lock (_lock) {
          if (_instance == null) {
            T[] _instances = FindObjectsOfType<T>();

            if (_instances.Length > 0) {
              _instance = _instances[0];

              if (_instances.Length > 1) {
                throw new System.Exception(string.Format("Application contains more than one (1) _instance of [Singleton] '{0}'. Reopen scene or stop creating multiple _instances to correct the issue", _instance));
              }
            } else {
              GameObject singleton = Resources.Load<GameObject>("Prefabs/" + typeof(T).Name);
              if (singleton == null) {
                singleton = Resources.Load<GameObject>("Prefabs/" + StringUtils.ToSpacedFormat(typeof(T).Name));
              }

              if (singleton != null) {
                singleton = Instantiate(singleton);
                _instance = singleton.GetComponent<T>();

                Debug.Log(string.Format("[Singleton] _instance of '{0}' was loaded from resources", typeof(T).Name));
              } else {
                singleton = new GameObject();
                _instance = singleton.AddComponent<T>();

                singleton.name = string.Format("(Singleton) {0}", typeof(T).Name);

                Debug.Log(string.Format("[Singleton] _instance of '{0}' was created", typeof(T).Name));
              }

              singleton.SetActive(true);

              DontDestroyOnLoad(singleton);
            }
          }

          return _instance;
        }
      }
    }
    #endregion

    #region Utility Functions
    private void OnApplicationQuit() {
      _isClosing = true;
    }


    private void OnDestroy() {
      _isClosing = true;
    }
    #endregion

  }

}
