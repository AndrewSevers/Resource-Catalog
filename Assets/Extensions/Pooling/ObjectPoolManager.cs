using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Pooling {

    /// <summary>
    /// Manager to handle and contain all object pools
    /// </summary>
    public class ObjectPoolManager : FiniteSingleton<ObjectPoolManager> {
		private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

		#region Pools
		/// <summary>
		/// Add an Object Pool to the map of pools using the GameObject's name as a key
		/// </summary>
		public void Add(PoolableObject aObject, Pool aObjectPool) {
			pools[aObject.ID] = aObjectPool;
		}

		/// <summary>
		/// Add an Object Pool to the map of pools using the string as a key
		/// </summary>
		public void Add(string aKey, Pool aObjectPool) {
			pools[aKey] = aObjectPool;
		}

		/// <summary>
		/// Get the pool attached to the GameObject's name (key)
		/// </summary>
		public Pool Get(PoolableObject aObject) {
			return Get(aObject.ID);
		}

		/// <summary>
		/// Get the pool associated with the given key
		/// </summary>
		public Pool Get(string aKey) {
			Pool pool;

			if (pools.TryGetValue(aKey, out pool) == false) {
				Debug.LogWarning(string.Format("Pool with key '{0}' does not exist within the pools mapping", aKey));
			}

			return pool;
		}

		/// <summary>
		/// Remove the pool associated to the GameObject's name (key)
		/// </summary>
		public void Remove(PoolableObject aObject) {
			pools.Remove(aObject.ID);
		}

		/// <summary>
		/// Remove the pool associated with the given key
		/// </summary>
		public void Remove(string aKey) {
			pools.Remove(aKey);
		}
		#endregion

		#region Utility Functions
		public void Clear() {
			foreach (ObjectPool<PoolableObject> pool in pools.Values) {
				pool.Destroy();
			}

			pools.Clear();
		}
		#endregion

	}

}
