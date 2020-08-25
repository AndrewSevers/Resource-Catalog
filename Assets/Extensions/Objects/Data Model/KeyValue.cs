using UnityEngine;

namespace Extensions {

  public struct KeyValue {
    private string _key;
    private object _value;

    #region Getters & Setters
    public string Key {
      get { return _key; }
    }

    public object Value {
      get { return _value; }
    }
    #endregion

    #region Constructor
    public KeyValue(string key, object value) {
      _key = key;
      _value = value;
    }
    #endregion
  }

}
