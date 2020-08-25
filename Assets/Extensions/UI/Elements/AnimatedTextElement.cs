using UnityEngine;

namespace Extensions.UI {

  [RequireComponent(typeof(Animator))]
  public class AnimatedTextElement : TextElement {
    private Animator animator = null;

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
    /// Overwrite the existing format with the one provided
    /// </summary>
    /// <param name="aFormat">Format to set</param>
    public override void SetFormat(string aFormat) {
      base.SetFormat(aFormat);
      animator.SetTrigger("Overwritten");
    }
    #endregion

  }

}
