using Resource.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Resource.UI {

    public class TextElement : MonoBehaviour {
        [SerializeField]
        private bool useExternalText = false;
        [SerializeField, Visibility("useExternalText", true)]
        private Text field;
        [SerializeField, Tooltip("Default format to use when updating the text field")]
		private string defaultFormat;

        #region Getters & Setters
        public bool UseExternalText {
            get { return useExternalText; }
        }

        public string Text {
			get { return field.text; }
		}
		#endregion

		#region Initialization
		private void Awake() {
			Initialize();
		}

		public virtual void Initialize() {
            if (useExternalText == false) {
                field = GetComponent<Text>();
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg">New text value</param>
        public virtual void UpdateText(object aArg) {
			field.text = string.Format(defaultFormat, aArg);
		}

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg1">New text value</param>
        /// <param name="aArg1">New text value</param>
        public virtual void UpdateText(object aArg1, object aArg2) {
            field.text = string.Format(defaultFormat, aArg1, aArg2);
        }

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArgs">New text value</param>
        public virtual void UpdateText(params object[] aArgs) {
            field.text = string.Format(defaultFormat, aArgs);
        }

        /// <summary>
        /// Overwrite the current text field with a new value
        /// </summary>
        /// <param name="aText">New text value</param>
        public virtual void OverwriteText(string aText) {
			field.text = aText;
		}

		/// <summary>
		/// Overwrite the current text field with the provided formatted text
		/// </summary>
		/// <param name="aFormatText">Format of displayed text</param>
		/// <param name="aValue">Int value to place into the formatted text</param>
		public virtual void OverwriteText(string aFormatText, int aValue) {
			field.text = string.Format(aFormatText, aValue);
		}

		/// <summary>
		/// Overwrite the current text field with the provided formatted text
		/// </summary>
		/// <param name="aFormatText">Format of displayed text</param>
		/// <param name="aValue">Float value to place into the formatted text</param>
		public virtual void OverwriteText(string aFormatText, float aValue) {
			field.text = string.Format(aFormatText, aValue);
		}
		#endregion

	}

}
