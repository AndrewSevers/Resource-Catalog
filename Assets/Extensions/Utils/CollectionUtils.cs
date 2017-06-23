using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Extensions.Utils {

	public static class CollectionUtils {

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

	}

}
