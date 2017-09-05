using UnityEngine;

namespace Extensions {

    public struct KeyValue {
        private string key;
        private object value;

        #region Getters & Setters
        public string Key {
            get { return key; }
        }

        public object Value {
            get { return value; }
        }
        #endregion

        #region Constructor
        public KeyValue(string aKey, object aValue) {
            key = aKey;
            value = aValue;
        }
        #endregion
    }

}
