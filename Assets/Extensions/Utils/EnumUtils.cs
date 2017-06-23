using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Extensions.Utils {

	public static class EnumUtils {
		public static bool HasFlag(this Enum value, Enum values) {
			return (Convert.ToUInt64(value) & Convert.ToUInt64(values)) == Convert.ToUInt64(values);
		}

		public static IEnumerable<T> GetFlags<T>(this Enum value) where T : struct {
			return GetFlags(value, (T[]) Enum.GetValues(typeof(T)));
		}

		public static IEnumerable<T> GetIndividualFlags<T>(this Enum value) where T : struct {
			return GetFlags(value, new List<T>(GetFlagValues<T>(value.GetType())).ToArray());
		}

		private static IEnumerable<T> GetFlags<T>(Enum value, T[] values) where T : struct {
			ulong bits = Convert.ToUInt64(value);
			List<T> results = new List<T>();
			for (int i = values.Length - 1; i >= 0; i--) {
				ulong mask = Convert.ToUInt64(values[i]);
				if (i == 0 && mask == 0L)
					break;
				if ((bits & mask) == mask) {
					results.Add(values[i]);
					bits -= mask;
				}
			}
			if (bits != 0L) {
				return new List<T>();
			}

			if (Convert.ToUInt64(value) != 0L) {
				results.Reverse();
				return results;
			}

			if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L) {
				return results.GetRange(0, 1);
			}

			return new List<T>();
		}

		private static IEnumerable<T> GetFlagValues<T>(Type enumType) where T : struct {
			ulong flag = 0x1;
			foreach (var value in Enum.GetValues(enumType)) {
				ulong bits = Convert.ToUInt64(value);
				if (bits == 0L) {
					continue; // skip the zero value
				}

				while (flag < bits) {
					flag <<= 1;
				}

				if (flag == bits) {
					yield return (T) value;
				}
			}
		}

	}

}
