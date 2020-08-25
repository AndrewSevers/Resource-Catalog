using UnityEngine;

namespace Extensions.Properties {

  /// <summary>
  /// Only allow the value to be within the contrained bounds and display in the inspector as a multi-bounded slider for all vector values
  /// </summary>
  public class LimitedVector3Attribute : PropertyAttribute {
    private Vector3 _min, _max;

    #region Getters & Setters
    public Vector3 Min {
      get { return _min; }
      set { _min = value; }
    }

    public Vector3 Max {
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
    /// <param name="xMax">Minimum x value the vector can be</param>
    /// <param name="yMin">Maximum y value the vector can be</param>
    /// <param name="yMax">Maximum y value the vector can be</param>
    /// <param name="zMin">Maximum z value the vector can be</param>
    /// <param name="zMax">Maximum z value the vector can be</param>
    public LimitedVector3Attribute(float xMin = 0.0f, float xMax = 100.0f, float yMin = 0.0f, float yMax = 100.0f, float zMin = 0.0f, float zMax = 100.0f) {
      _min = new Vector3(xMin, yMin, zMin);
      _max = new Vector3(xMax, yMax, zMax);
    }

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    /// <param name="min">Minimum value the vector can be</param>
    /// <param name="max">Maximum value the vector can be</param>
    public LimitedVector3Attribute(Vector3 min, Vector3 max) {
      _min = min;
      _max = max;
    }
    #endregion

  }

  #region Limited Range Class
  [System.Serializable]
  public struct LimitedVector3 {
    [SerializeField]
    private Vector3 _min, _max;

    #region Statics
    private static LimitedVector3 ten = new LimitedVector3(Vector3.zero, new Vector3(10.0f, 10.0f, 100.0f));
    private static LimitedVector3 hundred = new LimitedVector3(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));
    #endregion

    #region Getters & Setters
    public Vector3 Min {
      get { return _min; }
      set { _min = value; }
    }

    public Vector3 Max {
      get { return _max; }
      set { _max = value; }
    }

    public static LimitedVector3 ToTen {
      get { return ten; }
    }

    public static LimitedVector3 ToHundred {
      get { return hundred; }
    }
    #endregion

    #region Constructors
    public LimitedVector3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax) {
      _min = new Vector3(xMin, yMin, zMin);
      _max = new Vector3(xMax, yMax, zMax);
    }

    public LimitedVector3(Vector3 aMin, Vector3 aMax) {
      _min = aMin;
      _max = aMax;
    }
    #endregion

    #region Utility Functions
    /// <summary>
    /// Check to see if the given value is within the limited range
    /// </summary>
    /// <param name="aValue"></param>
    public bool WithinRange(Vector3 value) {
      return (value.x <= _max.x && value.x >= _min.x && value.y <= _max.y && value.y >= _min.y && value.z <= _max.z && value.z >= _min.z);
    }

    /// <summary>
    /// Check to see if the given values are within the limited range. If they are not the referenced index will contain the first invalid index.
    /// </summary>
    /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
    /// <param name="aValues">Values to compare</param>
    public bool WithinRange(ref int index, params Vector3[] values) {
      index = -1;

      for (int i = 0; i < values.Length; i++) {
        Vector3 value = values[i];
        if (value.x > _max.x || value.x < _min.x || value.y > _max.y || value.y < _min.y || value.z > _max.z || value.z < _min.z) {
          index = i;
          break;
        }
      }

      return (index == -1);
    }

    /// <summary>
    /// Get a random value within the min/max range
    /// </summary>
    public Vector3 Random() {
      if (_min != _max) {
        return new Vector3(UnityEngine.Random.Range(_min.x, _max.x), UnityEngine.Random.Range(_min.y, _max.y), UnityEngine.Random.Range(_min.z, _max.z));
      } else {
        return _min;
      }
    }
    #endregion

  }
  #endregion
}
