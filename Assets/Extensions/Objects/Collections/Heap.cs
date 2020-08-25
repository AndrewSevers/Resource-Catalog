using UnityEngine;
using System.Collections;
using System;

namespace Extensions.Collections {

  /// <summary>
  /// Heap collection type in which the contents are binary sorted with the first item being the greatest considering the provided comparator
  /// </summary>
  public class Heap<T> where T : IHeapItem<T> {
    private T[] _items = null;
    private int _size = 0;

    #region Getters & Setters
    /// <summary>Numbers of items currently in the heap</summary>
    public int Count {
      get { return _size; }
    }

    /// <summary>Length of the heap (regardless of fill status)</summary>
    public int Length {
      get { return _items.Length; }
    }

    public T[] Items {
      get { return _items; }
    }
    #endregion

    #region Constructor
    public Heap(int maxHeapSize) {
      _items = new T[maxHeapSize];
    }
    #endregion

    #region Item Management
    public void Add(T item) {
      item.HeapIndex = _size;
      _items[_size] = item;
      SortUp(item);
      _size++;
    }

    public T RemoveFirst() {
      T firstItem = _items[0];
      _size--;
      _items[0] = _items[_size];
      _items[0].HeapIndex = 0;
      SortDown(_items[0]);
      return firstItem;
    }

    public void UpdateItem(T item) {
      SortUp(item);
    }
    #endregion

    #region Sorting
    private void SortUp(T item) {
      int parentIndex = (item.HeapIndex - 1) / 2;

      while (true) {
        T parentItem = _items[parentIndex];
        if (item.CompareTo(parentItem) > 0) {
          Swap(item, parentItem);
        } else {
          break;
        }

        parentIndex = (item.HeapIndex - 1) / 2;
      }
    }

    private void SortDown(T item) {
      while (true) {
        int childIndexLeft = item.HeapIndex * 2 + 1;
        int childIndexRight = item.HeapIndex * 2 + 2;
        int swapIndex = 0;

        if (childIndexLeft < _size) {
          swapIndex = childIndexLeft;

          if (childIndexRight < _size) {
            if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0) {
              swapIndex = childIndexRight;
            }
          }

          if (item.CompareTo(_items[swapIndex]) < 0) {
            Swap(item, _items[swapIndex]);
          } else {
            return;
          }
        } else {
          return;
        }
      }
    }

    private void Swap(T a, T b) {
      _items[a.HeapIndex] = b;
      _items[b.HeapIndex] = a;
      int aIndex = a.HeapIndex;
      a.HeapIndex = b.HeapIndex;
      b.HeapIndex = aIndex;
    }
    #endregion

    #region Utility Functions
    public bool Contains(T item) {
      return Equals(_items[item.HeapIndex], item);
    }
    #endregion

    #region Cleanup
    public void Clear() {
      if (_size > 0) {
        Array.Clear(_items, 0, _items.Length);
        _size = 0;
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
