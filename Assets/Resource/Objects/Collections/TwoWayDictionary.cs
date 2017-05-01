using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Resource.Utils {

	/// <summary>
	/// TwTwo-Wayictionary Class
	/// </summary>
	/// <typeparam name="K">Key</typeparam>
	/// <typeparam name="V">Value</typeparam>
	public class TwoWayDictionary<K, V> : IDictionary<K, V> {
		private Dictionary<K, V> dictionary;
		private Dictionary<V, K> dictionaryReverse;

		#region Constructor
		public TwoWayDictionary(int aAmount) {
			dictionary = new Dictionary<K, V>(aAmount);
			dictionaryReverse = new Dictionary<V, K>(aAmount);
		}
		#endregion

		#region Base Accessors
		/// <summary>
		/// Associate the value to the given key
		/// </summary>
		/// <param name="aKey">Key to associate the value with</param>
		public V this[K aKey] {
			get { return dictionary[aKey]; }

			set {
				dictionary[aKey] = value;
				dictionaryReverse[value] = aKey;
			}
		}

		/// <summary>
		/// Associate the key to the given value
		/// </summary>
		/// <param name="aValue">Value to associate the key with</param>
		public K this[V aValue] {
			get { return dictionaryReverse[aValue]; }

			set {
				dictionaryReverse[aValue] = value;
				dictionary[value] = aValue;
			}
		}

		public ICollection<K> Keys {
			get { return dictionary.Keys; }
		}

		public ICollection<V> Values {
			get { return dictionary.Values; }
		}

		public int Count {
			get { return dictionary.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		#endregion

		#region Core Functions
		public void Add(K aKey, V aValue) {
			dictionary[aKey] = aValue;
			dictionaryReverse[aValue] = aKey;
		}

		public void Add(KeyValuePair<K, V> item) {
			throw new NotImplementedException();
		}

		public void Clear() {
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<K, V> item) {
			throw new NotImplementedException();
		}

		public bool ContainsKey(K key) {
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Remove the key-value pair from the dictionary based on the given key
		/// </summary>
		public void Remove(K aKey) {
			V value = dictionary[aKey];

			dictionary.Remove(aKey);
			dictionaryReverse.Remove(value);
		}

		/// <summary>
		/// Remove key-value pair from the dictionary based on the given value
		/// </summary>
		public void Remove(V aValue) {
			K key = dictionaryReverse[aValue];

			dictionary.Remove(key);
			dictionaryReverse.Remove(aValue);
		}

		public bool Remove(KeyValuePair<K, V> item) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the key for the given value if it exists in the dictionary
		/// </summary>
		public bool TryGetKey(V aValue, out K aKey) {
			return dictionaryReverse.TryGetValue(aValue, out aKey);
		}

		public bool TryGetValue(K key, out V value) {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		bool IDictionary<K, V>.Remove(K key) {
			throw new NotImplementedException();
		}
		#endregion

	}

}
