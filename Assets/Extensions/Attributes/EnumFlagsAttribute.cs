using UnityEngine;
using System.Collections;

namespace Extensions.Properties {

  /// <summary>
  /// Display the enum as a bitmask flags value that supports multiple enum values being selected at a single time
  /// </summary>
  public class EnumFlagsAttribute : PropertyAttribute {
    public EnumFlagsAttribute() { }
  }

}
