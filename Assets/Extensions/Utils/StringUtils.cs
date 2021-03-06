using System.Text;

namespace Extensions.Utils {

  /// <summary>
  /// Utilites relating to the usage of strings
  /// </summary>
  public static class StringUtils {
    public const char SPACE = ' ';
    public const string NONE = "None";

    #region Formatting
    /// <summary>
    /// Format the given string to match Unity's inspector field names (ex. rateOfFire => Rate Of Fire)
    /// </summary>
    /// <param name="aText">Text to convert to inspector case</param>
    public static string ToInspectorCase(this string aText) {
      StringBuilder newLabel = new StringBuilder(aText.Length);
      newLabel.Append(aText[0]);

      // Add a space before each capital letter in the text unless the capitalized letter is followed by another capitalized letter (AI, GUI, etc)
      for (int i = 1; i < aText.Length; i++) {
        newLabel.Append(aText[i]);

        if (i + 1 < aText.Length && char.IsUpper(aText[i + 1]) && char.IsLower(aText[i])) {
          newLabel.Append(SPACE);
        }
      }

      // Capitalize the first character of the text
      newLabel[0] = char.ToUpper(newLabel[0]);

      return newLabel.ToString();
    }

    /// <summary>
    /// Format the given string to add spaces between each capital letter with an option to capitalize the leading character (ex. ReadOnlyAttributeDrawer => Read Only Attribute Drawer)
    /// </summary>
    /// <param name="aText">Text to convert</param>
    /// <param name="aCapitalizeLeadingCharacter">Whether or not the first leading character should be capitalized</param>
    public static string ToSpacedFormat(this string aText, bool aCapitalizeLeadingCharacter = false) {
      StringBuilder newLabel = new StringBuilder(aText.Length);
      newLabel.Append(aText[0]);

      // Add a space before each capital letter in the text unless the capitalized letter is followed by another capitalized letter (AI, GUI, etc)
      for (int i = 1; i < aText.Length; i++) {
        newLabel.Append(aText[i]);

        if (i + 1 < aText.Length && char.IsUpper(aText[i + 1]) && char.IsLower(aText[i])) {
          newLabel.Append(SPACE);
        }
      }

      // Capitalize the first character of the text
      if (aCapitalizeLeadingCharacter) {
        newLabel[0] = char.ToUpper(newLabel[0]);
      }

      return newLabel.ToString();
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Remove all the leading characters provided for the given string.
    /// Leading characters constitutes all those characters that appear before another character type appears
    /// </summary>
    /// <param name="aSource">Source string that requires leading character removal</param>
    /// <param name="aCharacterToRemove">Character to remove</param>
    public static string RemoveLeadingCharacters(this string aSource, char aCharacterToRemove) {
      StringBuilder builder = new StringBuilder(aSource);

      while (builder[0] == aCharacterToRemove) {
        builder.Remove(0, 1);
      }

      return builder.ToString();
    }

    /// <summary>
    /// Check to see if the provided string is contained within a source string without case being important
    /// </summary>
    /// <param name="aSource">Source string that may, or may not, contain the string to check</param>
    /// <param name="aToCheck">String to find within the source string</param>
    public static bool ContainsIgnoreCase(this string aSource, string aToCheck) {
      return (aSource.IndexOf(aToCheck, System.StringComparison.CurrentCultureIgnoreCase) != -1);
    }
    #endregion

  }

}
