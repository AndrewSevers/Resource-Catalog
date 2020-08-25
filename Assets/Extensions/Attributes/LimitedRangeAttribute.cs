using UnityEngine;

namespace Extensions.Properties {

  /// <summary>
  /// Only allow the value to be within the contrained bounds and display in the inspector as a multi-bounded slider
  /// </summary>
  public class LimitedRangeAttribute : PropertyAttribute {
    private float _min, _max;

    #region Getters & Setters
    public float Min {
      get { return _min; }
      set { _min = value; }
    }

    public float Max {
      get { return _max; }
      set { _max = value; }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    /// <param name="min">Minimum value the range can be</param>
    /// <param name="max">Maximum value the range can be</param>
    public LimitedRangeAttribute(float min = 0, float max = 100) {
      _min = min;
      _max = max;
    }
    #endregion

  }

  #region Limited Range Class
  [System.Serializable]
  public struct LimitedRange {
    [SerializeField]
    private float _min, _max;

    #region Statics
    private static LimitedRange ten = new LimitedRange(0.0f, 10.0f);
    private static LimitedRange hundred = new LimitedRange(0.0f, 100.0f);
    #endregion

    #region Getters & Setters
    public float Min {
      get { return _min; }
      set { _min = value; }
    }

    public int MinToInt {
      get { return (int) _min; }
    }

    public float Max {
      get { return _max; }
      set { _max = value; }
    }

    public int MaxToInt {
      get { return (int) _max; }
    }

    public static LimitedRange ToTen {
      get { return ten; }
    }

    public static LimitedRange ToHundred {
      get { return hundred; }
    }
    #endregion

    #region Constructors
    public LimitedRange(float min, float max) {
      _min = min;
      _max = max;
    }
    #endregion

    #region Utility Functions
    /// <summary>
    /// Check to see if the given value is within the clamped range
    /// </summary>
    /// <param name="aValue"></param>
    public bool WithinRange(float value) {
      return (value <= _max && value >= _min);
    }

    /// <summary>
    /// Check to see if the given values are within the clamped range. If they are not the referenced index will contain the first invalid index.
    /// </summary>
    /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
    /// <param name="aValues">Values to compare</param>
    public bool WithinRange(ref int index, params float[] values) {
      index = -1;

      for (int i = 0; i < values.Length; i++) {
        float value = values[i];
        if (value > _max || value < _min) {
          index = i;
          break;
        }
      }

      return (index == -1);
    }

    /// <summary>
    /// Get a random value within the min/max range
    /// </summary>
    public float Random() {
      if (_min != _max) {
        return UnityEngine.Random.Range(_min, _max);
      } else {
        return _min;
      }
    }

    /// <summary>
    /// Get a random int value within the min/max range
    /// </summary>
    public int RandomToInt() {
      if (_min != _max) {
        return Mathf.RoundToInt(UnityEngine.Random.Range(_min, _max));
      } else {
        return Mathf.RoundToInt(_min);
      }
    }
    #endregion

  }
  #endregion

}
