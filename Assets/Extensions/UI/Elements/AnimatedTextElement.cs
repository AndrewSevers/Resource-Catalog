using UnityEngine;

namespace Extensions.UI {

    [RequireComponent(typeof(Animator))]
	public class AnimatedTextElement : TextElement {
		private Animator animator;

		#region Getters & Setters
		public Animator Animator {
			get { return animator; }
		}
		#endregion

		#region Initialization
		private void Awake() {
			Initialize();
		}

		public override void Initialize() {
            base.Initialize();

			animator = GetComponent<Animator>();
		}
        #endregion

        #region Update
        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg">New text value</param>
        public override void UpdateText(object aArg) {
            base.UpdateText(aArg);
			animator.SetTrigger("Updated");
		}

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArg1">Int value to place into the formatted text</param>
        /// <param name="aArg2">Int value to place into the formatted text</param>
        public override void UpdateText(object aArg1, object aArg2) {
            base.UpdateText(aArg1, aArg2);
            animator.SetTrigger("Updated");
		}

        /// <summary>
        /// Update the text field using the default formatted text
        /// </summary>
        /// <param name="aArgs">Float value to place into the formatted text</param>
        public override void UpdateText(params object[] aArgs) {
            base.UpdateText(aArgs);
            animator.SetTrigger("Updated");
		}

		/// <summary>
		/// Overwrite the current text field with a new value
		/// </summary>
		/// <param name="aText">New text value</param>
		public override void OverwriteText(string aText) {
            base.OverwriteText(aText);
            animator.SetTrigger("Overwritten");
		}

		/// <summary>
		/// Overwrite the current text field with the provided formatted text
		/// </summary>
		/// <param name="aFormatText">Format of displayed text</param>
		/// <param name="aValue">Int value to place into the formatted text</param>
		public override void OverwriteText(string aFormatText, int aValue) {
            base.OverwriteText(aFormatText, aValue);
            animator.SetTrigger("Overwritten");
		}

		/// <summary>
		/// Overwrite the current text field with the provided formatted text
		/// </summary>
		/// <param name="aFormatText">Format of displayed text</param>
		/// <param name="aValue">Float value to place into the formatted text</param>
		public override void OverwriteText(string aFormatText, float aValue) {
            base.OverwriteText(aFormatText, aValue);
            animator.SetTrigger("Overwritten");
		}
		#endregion

	}

}
