using System.Collections.Generic;

namespace Extensions.Math {

    public class Fibonnaci {
        private static Fibonnaci sequence;

        private List<int> numbers = new List<int>();

        #region Getters & Setters
        public static Fibonnaci Sequence {
            get {
                if (sequence == null) {
                    sequence = new Fibonnaci();
                }

                return sequence;
            }
        }

        public int this[int aIndex] {
            get { return GetNumber(aIndex); }
        }
        #endregion

        #region Constructor
        private Fibonnaci() {
            numbers.Add(0);
            numbers.Add(1);
        }
        #endregion

        #region Utility Functions
        public int GetNumber(int aIndex) {
            if (aIndex < 0) {
                throw new System.ArgumentOutOfRangeException("aIndex must be positive");
            }

            if (aIndex >= numbers.Count) {
                GenerateUpTo(aIndex);
            }

            return numbers[aIndex];
        }

        private void GenerateUpTo(int aIndex) {
            for (int i = numbers.Count - 1; i < aIndex; i++) {
                numbers.Add(numbers[i - 1] + numbers[i]);
            }
        }
        #endregion

        #region Cleanup
        public void Clear() {
            sequence = null;
        }
        #endregion

    }

}
