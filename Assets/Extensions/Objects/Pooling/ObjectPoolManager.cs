using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Pooling {

  /// <summary>
  /// Manager to handle and contain all object pools
  /// </summary>
  public class ObjectPoolManager : FiniteSingleton<ObjectPoolManager> {
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    #region Pools
    /// <summary>
    /// Add an Object Pool to the map of pools using the PoolableObject's id as a key
    /// </summary>
    public void Add(PoolableObject poolableObject, Pool pool) {
      _pools[poolableObject.ID] = pool;
    }

    /// <summary>
    /// Add an Object Pool to the map of pools using an id as a key
    /// </summary>
    public void Add(string id, Pool pool) {
      _pools[id] = pool;
    }

    /// <summary>
    /// Get the pool attached to the PoolableObject's id
    /// </summary>
    public Pool Get(PoolableObject poolableObject) {
      return Get(poolableObject.ID);
    }

    /// <summary>
    /// Get the pool associated with the given id
    /// </summary>
    public Pool Get(string id) {
      if (_pools.TryGetValue(id, out Pool pool) == false) {
        Debug.LogWarning(string.Format("Pool with id '{0}' does not exist within the pools mapping", id));
      }

      return pool;
    }

    /// <summary>
    /// Remove the pool associated to the PoolableObject's id
    /// </summary>
    public void Remove(PoolableObject poolableObject) {
      _pools.Remove(poolableObject.ID);
    }

    /// <summary>
    /// Remove the pool associated with the given id
    /// </summary>
    public void Remove(string id) {
      _pools.Remove(id);
    }
    #endregion

    #region Utility Functions
    /// <summary>
    /// Clear (destroy) all pools
    /// </summary>
    public void Clear() {
      foreach (ObjectPool<PoolableObject> pool in _pools.Values) {
        pool.Destroy();
      }

      _pools.Clear();
    }
    #endregion

  }

}
