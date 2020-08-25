using UnityEngine;

namespace Extensions.Properties {

  /// <summary>
  /// Only allow the value to be within the contrained bounds and display in the inspector as a multi-bounded slider for all vector values
  /// </summary>
  public class LimitedVector2Attribute : PropertyAttribute {
    private Vector2 _min, _max;

    #region Getters & Setters
    public Vector2 Min {
      get { return _min; }
      set { _min = value; }
    }

    public Vector2 Max {
      get { return _max; }
      set { _max = value; }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    /// <param name="xMin">Minimum x value the vector can be</param>
    /// <param name="xMax">Minimum y value the vector can be</param>
    /// <param name="xMax">Maximum x value the vector can be</param>
    /// <param name="yMax">Maximum y value the vector can be</param>
    public LimitedVector2Attribute(float xMin = 0.0f, float xMax = 100.0f, float yMin = 0.0f, float yMax = 100.0f) {
      _min = new Vector2(xMin, yMin);
      _max = new Vector2(xMax, yMax);
    }

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    /// <param name="min">Minimum value the vector can be</param>
    /// <param name="max">Maximum value the vector can be</param>
    public LimitedVector2Attribute(Vector2 min, Vector2 max) {
      _min = min;
      _max = max;
    }
    #endregion

  }

  #region Clamped Range Class
  [System.Serializable]
  public struct LimitedVector2 {
    [SerializeField]
    private Vector2 _min, _max;

    #region Statics
    private static LimitedVector2 ten = new LimitedVector2(Vector2.zero, new Vector2(10.0f, 10.0f));
    private static LimitedVector2 hundred = new LimitedVector2(Vector2.zero, new Vector2(100.0f, 100.0f));
    #endregion

    #region Getters & Setters
    public Vector2 Min {
      get { return _min; }
      set { _min = value; }
    }

    public Vector2 Max {
      get { return _max; }
      set { _max = value; }
    }

    public static LimitedVector2 ToTen {
      get { return ten; }
    }

    public static LimitedVector2 ToHundred {
      get { return hundred; }
    }
    #endregion

    #region Constructors
    public LimitedVector2(float xMin, float yMin, float xMax, float yMax) {
      _min = new Vector2(xMin, yMin);
      _max = new Vector2(xMax, yMax);
    }

    public LimitedVector2(Vector2 min, Vector2 max) {
      _min = min;
      _max = max;
    }
    #endregion

    #region Utility Functions
    /// <summary>
    /// Check to see if the given value is within the limited range
    /// </summary>
    /// <param name="value"></param>
    public bool WithinRange(Vector2 value) {
      return (value.x <= _min.x && value.x >= _min.x && value.y <= _min.y && value.y >= _min.y);
    }

    /// <summary>
    /// Check to see if the given values are within the limited range. If they are not the referenced index will contain the first invalid index.
    /// </summary>
    /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
    /// <param name="aValues">Values to compare</param>
    public bool WithinRange(ref int index, params Vector2[] values) {
      index = -1;

      for (int i = 0; i < values.Length; i++) {
        Vector2 value = values[i];
        if (value.x > _min.x || value.x < _min.x || value.y > _min.y || value.y < _min.y) {
          index = i;
          break;
        }
      }

      return (index == -1);
    }

    /// <summary>
    /// Get a random value within the min/max range
    /// </summary>
    public Vector2 Random() {
      if (_min != _max) {
        return new Vector2(UnityEngine.Random.Range(_min.x, _min.x), UnityEngine.Random.Range(_min.y, _min.y));
      } else {
        return _min;
      }
    }
    #endregion

  }
  #endregion

}
