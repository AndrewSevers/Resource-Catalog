using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.Utils {

  public static class CollectionUtils {

    #region Reversal of Collection
    public static void ReverseNonAlloc<T>(this List<T> list) {
      ReverseNonAlloc(list.ToArray());
    }

    public static void ReverseNonAlloc<T>(this T[] array) {
      int count = array.Length;

      for (int i = 0; i < count / 2; i++) {
        T tmp = array[i];
        array[i] = array[count - i - 1];
        array[count - i - 1] = tmp;
      }
    }
    #endregion

    #region Conversion
    /// <summary>
    /// Convert an amount of KeyValue pairs into a dictionary
    /// </summary>
    /// <param name="data">Data to convert</param>
    public static Dictionary<string, object> ToDictionary(params KeyValue[] data) {
      return data.ToDictionary(item => item.Key, item => item.Value);
    }
    #endregion

  }

}
