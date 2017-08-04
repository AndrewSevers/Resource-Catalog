using Extensions.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.UI {

    public class TextElement : MonoBehaviour {
        [SerializeField, Tooltip("Format to use when updating the text field")]
		private string format;

        private Text field;

        #region Getters & Setters
        public string Text {
			get { return field.text; }
		}
		#endregion

		#region Initialization
		private void Awake() {
			Initialize();
		}

		public virtual void Initialize() {
            field = GetComponent<Text>();
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg">New text value</param>
        public virtual void UpdateText(object aArg) {
			field.text = string.Format(format, aArg);
		}

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg1">New text value</param>
        /// <param name="aArg1">New text value</param>
        public virtual void UpdateText(object aArg1, object aArg2) {
            field.text = string.Format(format, aArg1, aArg2);
        }

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArgs">New text value</param>
        public virtual void UpdateText(params object[] aArgs) {
            field.text = string.Format(format, aArgs);
        }

        /// <summary>
        /// Overwrite the existing format with the one provided
        /// </summary>
        /// <param name="aFormat">Format to set</param>
        public virtual void SetFormat(string aFormat) {
			format = aFormat;
		}

        /// <summary>
        /// Clear the format so it is blank
        /// </summary>
        public virtual void ClearFormat() {
            format = string.Empty;
        }
		#endregion

	}

}
