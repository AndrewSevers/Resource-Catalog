using UnityEngine;
using System.Collections;
using System;

namespace Resource.Collections {

	/// <summary>
	/// Heap collection type in which the contents are binary sorted with the first item being the greatest considering the provided comparator
	/// </summary>
	public class Heap<T> where T : IHeapItem<T> {
		private T[] items;
		private int size = 0;

		#region Getters & Setters
		/// <summary>Numbers of items currently in the heap</summary>
		public int Count {
			get { return size; }
		}

		/// <summary>Length of the heap (regardless of fill status)</summary>
		public int Length {
			get { return items.Length; }
		}

		public T[] Items {
			get { return items; }
		}
		#endregion

		#region Constructor
		public Heap(int aMaxHeapSize) {
			items = new T[aMaxHeapSize];
		}
		#endregion

		#region Item Management
		public void Add(T aItem) {
			aItem.HeapIndex = size;
			items[size] = aItem;
			SortUp(aItem);
			size++;
		}

		public T RemoveFirst() {
			T firstItem = items[0];
			size--;
			items[0] = items[size];
			items[0].HeapIndex = 0;
			SortDown(items[0]);
			return firstItem;
		}

		public void UpdateItem(T aItem) {
			SortUp(aItem);
		}
		#endregion

		#region Sorting
		private void SortUp(T aItem) {
			int parentIndex = (aItem.HeapIndex - 1) / 2;

			while (true) {
				T parentItem = items[parentIndex];
				if (aItem.CompareTo(parentItem) > 0) {
					Swap(aItem, parentItem);
				} else {
					break;
				}

				parentIndex = (aItem.HeapIndex - 1) / 2;
			}
		}

		private void SortDown(T aItem) {
			while (true) {
				int childIndexLeft = aItem.HeapIndex * 2 + 1;
				int childIndexRight = aItem.HeapIndex * 2 + 2;
				int swapIndex = 0;

				if (childIndexLeft < size) {
					swapIndex = childIndexLeft;

					if (childIndexRight < size) {
						if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
							swapIndex = childIndexRight;
						}
					}

					if (aItem.CompareTo(items[swapIndex]) < 0) {
						Swap(aItem, items[swapIndex]);
					} else {
						return;
					}
				} else {
					return;
				}
			}
		}

		private void Swap(T aItemA, T aItemB) {
			items[aItemA.HeapIndex] = aItemB;
			items[aItemB.HeapIndex] = aItemA;
			int itemAIndex = aItemA.HeapIndex;
			aItemA.HeapIndex = aItemB.HeapIndex;
			aItemB.HeapIndex = itemAIndex;
		}
		#endregion

		#region Utility Functions
		public bool Contains(T aItem) {
			return Equals(items[aItem.HeapIndex], aItem);
		}
		#endregion

		#region Cleanup
		public void Clear() {
			if (size > 0) {
				Array.Clear(items, 0, items.Length);
				size = 0;
			}
		}
		#endregion

	}

	public interface IHeapItem<T> : IComparable<T> {
		int HeapIndex {
			get;
			set;
		}
	}

}
