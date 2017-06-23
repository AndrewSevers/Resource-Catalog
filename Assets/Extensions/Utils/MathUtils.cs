using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Utils {

	public static class MathUtils {

		#region Fibonacci Series
		public static int GetFibonacciNumber(int aPositionInSequence) {
			int n1 = 0;
			int n2 = 1;

			for (int i = 1; i < aPositionInSequence; i++) {
				int value = n1 + n2;
				n1 = n2;
				n2 = value;
			}

			return n1;
		}

		public static List<int> GetFibonacciSeries(int aPositionInSequence) {
			List<int> numbers = new List<int>(aPositionInSequence);
			numbers.Add(0);
			numbers.Add(1);

			for (int i = 1; i < aPositionInSequence; i++) {
				numbers.Add(numbers[i - 1] + numbers[i]);
			}

			return numbers;
		}
		#endregion

		#region Digit Count
		/// <summary>
		/// Get the number of digits contained within the number (length of the number)
		/// </summary>
		public static int DigitCount(int aNumber) {
			return (aNumber == 0) ? 1 : (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(aNumber)) + 1);
		}

		/// <summary>
		/// Get the number of digits contained within the number (length of the number)
		/// </summary>
		public static int DigitCount(long aNumber) {
			return (aNumber == 0) ? 1 : (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(aNumber)) + 1);
		}

		/// <summary>
		/// Get the number of digits contained within the number (length of the number)
		/// </summary>
		public static int DigitCount(float aNumber) {
			string number = System.Math.Abs(aNumber).ToString().Replace(".", string.Empty);
			return (aNumber == 0) ? 1 : (int) System.Math.Floor(System.Math.Log10(float.Parse(number)) + 1);
		}

		/// <summary>
		/// Get the number of digits contained within the number (length of the number)
		/// </summary>
		public static int DigitCount(double aNumber) {
			string number = System.Math.Abs(aNumber).ToString().Replace(".", string.Empty);
			return (aNumber == 0) ? 1 : (int) System.Math.Floor(System.Math.Log10(float.Parse(number)) + 1);
		}
        #endregion

        #region Random
        /// <summary>
        /// Using an array of probablities return the index for the selected probablity
        /// </summary>
        /// <param name="aProbabilities">Probablities to select from</param>
        /// <param name="aSort">Should the array be sorted</param>
        /// <returns>Index of the probablity chosen</returns>
        public static int GetWeightedRandom(int[] aProbabilities, bool aSort = false) {
            if (aSort) {
                System.Array.Sort(aProbabilities, new System.Comparison<int>((a, b) => a.CompareTo(b)));
            }

            // Setup total weight value
            float totalWeight = 0;
            for (int i = 0; i < aProbabilities.Length; i++) {
                totalWeight += aProbabilities[i];
            }

            // Select a random value from all the weights and start with a weight of zero
            float randomWeight = Random.Range(0, totalWeight);
            float currentWeight = 0;

            // Check each value to see if it matches the specified weight
            int index = 0;
            for (index = 0; index < aProbabilities.Length; index++) {
                currentWeight += aProbabilities[index];
                if (currentWeight > randomWeight) {
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Using an array of probablities return the index for the selected probablity
        /// </summary>
        /// <param name="aProbabilities">Probablities to select from</param>
        /// <param name="aSort">Should the array be sorted</param>
        /// <returns>Index of the probablity chosen</returns>
        public static int GetWeightedRandom(float[] aProbabilities, bool aSort = false) {
            if (aSort) {
                System.Array.Sort(aProbabilities, new System.Comparison<float>((a, b) => a.CompareTo(b)));
            }

            // Setup total weight value
            float totalWeight = 0;
            for (int i = 0; i < aProbabilities.Length; i++) {
                Debug.Log(aProbabilities[i]);
                totalWeight = aProbabilities[i];
            }

            // Select a random value from all the weights and start with a weight of zero
            float randomWeight = Random.Range(0, totalWeight);
            float currentWeight = 0;

            // Check each value to see if it matches the specified weight
            int index = 0;
            for (index = 0; index < aProbabilities.Length; index++) {
                currentWeight += aProbabilities[index];
                if (currentWeight > randomWeight) {
                    break;
                }
            }

            return index;
        }
        #endregion

        #region Comparisons
        /// <summary>
        /// Determine whether the provided value is between the min and max values
        /// </summary>
        /// <param name="aValue">Value to compare</param>
        /// <param name="aMin">Min value</param>
        /// <param name="aMax">Max value</param>
        /// <param name="aMinInclusive">If the value can equal the min value</param>
        /// <param name="aMaxInclusive">If the value can equal the max value</param>
        public static bool Between(int aValue, int aMin, int aMax, bool aMinInclusive = true, bool aMaxInclusive = true) {
            if (aMinInclusive) {
                if (aValue < aMin) {
                    return false;
                }
            } else {
                if (aValue <= aMin) {
                    return false;
                }
            }

            if (aMaxInclusive) {
                if (aValue > aMax) {
                    return false;
                }
            } else {
                if (aValue >= aMax) {
                    return false;
                }
            }

            return true;
        }

        public static bool IsApproximately(float a, float b, float aTolerance = 0.01f) {
            return Mathf.Abs(a - b) < aTolerance;
        }
        #endregion

    }

}
