using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.Utils {

    public static class CollectionUtils {

        #region Reversal of Collection
        public static void ReverseNonAlloc<T>(this List<T> aList) {
            int count = aList.Count;

            for (int i = 0; i < count / 2; i++) {
                T tmp = aList[i];
                aList[i] = aList[count - i - 1];
                aList[count - i - 1] = tmp;
            }
        }

        public static void ReverseNonAlloc<T>(this T[] aArray) {
            int count = aArray.Length;

            for (int i = 0; i < count / 2; i++) {
                T tmp = aArray[i];
                aArray[i] = aArray[count - i - 1];
                aArray[count - i - 1] = tmp;
            }
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Convert an amount of KeyValue pairs into a dictionary
        /// </summary>
        /// <param name="aData">Data to convert</param>
        public static Dictionary<string, object> ToDictionary(params KeyValue[] aData) {
            return aData.ToDictionary(item => item.Key, item => item.Value);
        }
        #endregion

    }

}
