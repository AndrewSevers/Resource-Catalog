using System.Collections.Generic;

namespace Extensions.Collections {

	/// <summary>
	/// Multi-Dictionary Class
	/// </summary>
	/// <typeparam name="K">Key</typeparam>
	/// <typeparam name="V">Value</typeparam>
	public class MultiDictionary<K, V> {
		private Dictionary<K, List<V>> dictionary = null;

		#region Constructor
		public MultiDictionary(int size = 0) {
			dictionary = new Dictionary<K, List<V>>(size);
		}
		#endregion

		#region Base Accessor
		public List<V> this[K key] {
			get {
				if (dictionary.TryGetValue(key, out List<V> item)) {
					return item;
				}

				throw new KeyNotFoundException(string.Format("Key '{0}' not found within dictionary.", key));
			}

			set {
				dictionary[key] = value;
			}
		}
		#endregion

		#region Core Functions
		/// <summary>
		/// Add a single value to the list of values for the given key
		/// </summary>
		public void Add(K key, V value) {
			if (dictionary.TryGetValue(key, out List<V> items) == false) {
				items = new List<V>();
			}

			items.Add(value);
			dictionary[key] = items;
		}

		/// <summary>
		/// Add a group amount of values to the list of values for the given key
		/// </summary>
		public void Add(K key, params V[] values) {
			if (dictionary.TryGetValue(key, out List<V> items) == false) {
				items = new List<V>();
				dictionary[key] = items;
			}

			foreach (V value in values) {
				items.Add(value);
			}
		}

		/// <summary>
		/// Remove an entire key-value pair based on the given key
		/// </summary>
		public void Remove(K key) {
			dictionary.Remove(key);
		}

		/// <summary>
		/// Remove a single value from the given key
		/// </summary>
		public void Remove(K key, V value) {
			if (dictionary.TryGetValue(key, out List<V> items)) {
				items.Remove(value);
			}
		}

		/// <summary>
		/// Remove a group of values from the given key
		/// </summary>
		public void Remove(K key, params V[] values) {
			if (dictionary.TryGetValue(key, out List<V> items) == false) {
				items = new List<V>();
				dictionary[key] = items;
			}

			foreach (V value in values) {
				items.Remove(value);
			}
		}

		public bool TryGetValue(K key, out List<V> aValues) {
			aValues = default(List<V>);

			return dictionary.TryGetValue(key, out aValues);
		}
		#endregion

		#region Core Utilities
		public Dictionary<K, List<V>>.KeyCollection Keys {
			get {
				return dictionary.Keys;
			}
		}

		public Dictionary<K, List<V>>.ValueCollection Values {
			get {
				return dictionary.Values;
			}
		}

		/// <summary>
		/// Returns the number of key-value pairs contained within the dictionary
		/// </summary>
		public int Count {
			get {
				return dictionary.Count;
			}
		}

		/// <summary>
		/// Returns the number of values for the given key
		/// </summary>
		public int ValueCount(K key) {
			if (dictionary.TryGetValue(key, out List<V> items)) {
				return items.Count;
			}

			return 0;
		}

		/// <summary>
		/// Returns whether or not the given key is contained within the dictionaries keys
		/// </summary>
		public bool ContainsKey(K key) {
			return dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Remove all data from the dictionary
		/// </summary>
		public void Clear() {
			dictionary.Clear();
		}
		#endregion

	}

}
