using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Pooling {

  public class ObjectPool<T> : Pool where T : PoolableObject {
    private T prefab = null;
    private Stack<T> _objects = null;

    #region Getters & Setters
    public int Count {
      get { return _objects.Count; }
    }
    #endregion

    #region Constructor
    public ObjectPool(T aPrefab, int aAmount = 0) {
      Initialize(aPrefab, aAmount);
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialize and setup a pool with Gameobjects
    /// </summary>
    protected virtual void Initialize(T aPrefab, int aAmount = 0) {
      if (initialized == false) {
        initialized = true;

        container = new GameObject("[ObjectPool] " + aPrefab.name);
        _objects = new Stack<T>();

        prefab = aPrefab;

        ObjectPoolManager.Instance.Add(prefab, this);

        if (aAmount == 0) {
          aAmount = aPrefab.InitialPoolSize;
        }

        for (int i = 0; i < aAmount; i++) {
          CreateObject();
        }
      } else {
        Debug.LogWarning(string.Format("[ObjectPool] '{0}' already initialized. Avoid initializing pools more than once!", container.name));
      }
    }
    #endregion

    #region Creation
    /// <summary>
    /// Create a new GameObject for the pool
    /// </summary>
    protected virtual T CreateObject() {
      T newObject = Object.Instantiate(prefab);
      newObject.gameObject.SetActive(false);
      newObject.transform.SetParent(container.transform, false);

      // Initialize the poolable object
      PoolableObject newPoolableObject = newObject.GetComponent<PoolableObject>();
      if (newPoolableObject != null) {
        newPoolableObject.Initialize(this);
      } else {
        Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, typeof(PoolableObject)));
      }

      _objects.Push(newObject);

      return newObject;
    }
    #endregion

    #region Get Functions
    /// <summary>
    /// Get the next GameObject from the pool. If the pool is empty a new object will be created.
    /// </summary>
    public T GetObject() {
      if (_objects.Count == 0) {
        CreateObject();
      }

      T freedObject = _objects.Pop();
      freedObject.transform.SetParent(null, false);

      freedObject.OnFree();

      return freedObject;
    }

    /// <summary>
    /// Get the next GameObject from the pool. If the pool is empty a new object will be created. Start a timer to repool the object upone freeing it.
    /// </summary>
    /// <param name="aRepoolTime">Time until the object is automatically repooled after being freed from the pool</param>
    public T GetObject(float aRepoolTime) {
      if (_objects.Count == 0) {
        CreateObject();
      }

      T freedObject = _objects.Pop();
      freedObject.transform.SetParent(null, false);

      freedObject.OnFree(aRepoolTime);

      return freedObject;
    }
    #endregion

    #region Return Functions
    /// <summary>
    /// Return the GameObject to the pool so it can be re-used again
    /// </summary>
    /// <param name="aObject">Object to return to the pool</param>
    public override void Return(PoolableObject aObject) {
      if (_objects.Contains(aObject as T) == false) {
        aObject.transform.SetParent(container.transform, false);
        aObject.gameObject.SetActive(false);

        _objects.Push(aObject as T);
      }
    }
    #endregion

    #region Cleanup
    /// <summary>
    /// Destroy and clear all pooled objects (including container)
    /// </summary>
    public void Destroy() {
      Object.Destroy(container);
      _objects.Clear();
    }
    #endregion

  }

}
