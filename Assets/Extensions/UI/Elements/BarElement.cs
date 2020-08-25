using Extensions.Properties;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.UI {

  /// <summary>
  /// UI Element that can fade in and out (including all of its children)
  /// </summary>
  [RequireComponent(typeof(Image))]
  public class BarElement : MonoBehaviour {
    [SerializeField, Tooltip("Smooth out changes to values")]
    private bool smooth = false;
    [SerializeField, Visibility("smooth", true), Tooltip("Smooth rate when initialized")]
    private float initialSmoothRate = 1.0f;
    [SerializeField, Visibility("smooth", true)]
    private float smoothRate = 1.0f;

    [Header("Text Element")]
    [SerializeField]
    private bool showTextValue = false;
    [SerializeField, Visibility("showTextValue", true)]
    private bool showAsPercentage = false;
    [SerializeField, Visibility("showTextValue", true)]
    private Text valueField = null;
    [SerializeField, Visibility("showTextValue", true)]
    private string valueFormat = null;

    private Image content = null;
    private float currentValue = 0;
    private float maxValue = 0;
    private bool initialized = false;

    private Coroutine adjustCoroutine = null;
    private float coroutineTime = 0.0f;
    private float coroutineStartAmount = 0.0f;
    private float coroutineFinalAmount = 0.0f;

    #region Getters & Setters
    public float InitialSmoothRate {
      get { return initialSmoothRate; }
    }

    public float SmoothRate {
      get { return smoothRate; }
      set { smoothRate = value; }
    }

    public float MaxValue {
      get { return maxValue; }
      set { AdjustValue(currentValue, value); }
    }

    public Text ValueField {
      get { return valueField; }
    }

    public float Value {
      get { return currentValue; }
      set { AdjustValue(value); }
    }

    public float FillAmount {
      get { return content.fillAmount; }
      set { AdjustValue(maxValue * value); }
    }

    public bool IsFilled {
      get { return content.fillAmount == 1; }
    }
    #endregion

    #region Initialization
    public virtual void Initialize(float aInitialValue, float aMaxValue, float aSmoothRate = -1) {
      if (initialized == false) {
        content = GetComponent<Image>();

        if (content.type != Image.Type.Filled) {
          throw new System.Exception("[BarElement] Image type is not filled");
        }
      }

      currentValue = aInitialValue;
      maxValue = aMaxValue;

      if (aSmoothRate != -1) {
        smoothRate = aSmoothRate;
      }

      initialized = true;

    }
    #endregion

    #region Bar Management
    public virtual void AdjustValue(float aAmount, float aMaxValue = 0.0f, float aSmoothRate = 0.0f) {
      currentValue = aAmount;
      if (aMaxValue > 0) {
        maxValue = aMaxValue;
      }

      float amount = (aAmount / maxValue);

      // Adjust the bar
      if (smooth) {
        if (adjustCoroutine == null) {
          adjustCoroutine = StartCoroutine(SmoothAdjustment(amount, aSmoothRate, showTextValue));
        } else {
          UpdateAdjustments(amount);
        }
      } else {
        content.fillAmount = amount;

        // Adjust the text
        if (showTextValue) {
          AdjustText();
        }
      }
    }

    // Smoothly adjust the bar
    protected virtual IEnumerator SmoothAdjustment(float aAmount, float aSmoothRate = 0.0f, bool aUpdateText = false) {
      coroutineStartAmount = content.fillAmount;
      coroutineFinalAmount = aAmount;
      coroutineTime = 0;

      if (aSmoothRate == 0) {
        aSmoothRate = smoothRate;
      }

      while (coroutineTime < 1) {
        coroutineTime += (aSmoothRate * Time.deltaTime);
        content.fillAmount = Mathf.Lerp(coroutineStartAmount, coroutineFinalAmount, coroutineTime);

        if (aUpdateText) {
          AdjustText();
        }

        yield return null;
      }

      adjustCoroutine = null;
    }

    private void UpdateAdjustments(float aNewAmount) {
      coroutineStartAmount = content.fillAmount;
      coroutineFinalAmount = aNewAmount;
      coroutineTime = 0;
    }

    // Adjust the text to fit the format
    protected virtual void AdjustText() {
      if (valueField != null) {
        valueField.text = (showAsPercentage) ? string.Format(valueFormat + "%", Mathf.RoundToInt((content.fillAmount * 100))) : string.Format(valueFormat, currentValue);
      }
    }
    #endregion

    #region Utility Functions
    public void Fill(bool aAnimate = false, float aRate = 0.0f) {
      if (aAnimate) {
        content.fillAmount = 0;
        AdjustValue(maxValue, 0, aRate);
      } else {
        content.fillAmount = 1;
      }
    }

    public void Refill(bool aAnimate = false, float aRate = 0.0f) {
      if (aAnimate) {
        content.fillAmount = 0;
        AdjustValue(maxValue, 0, aRate);
      } else {
        content.fillAmount = 1;
      }
    }

    public void Empty(bool aAnimate = false, float aRate = 0.0f) {
      if (aAnimate) {
        content.fillAmount = 1;
        AdjustValue(0, maxValue, aRate);
      } else {
        content.fillAmount = 0;
      }
    }
    #endregion

    #region Cleanup
    public void OnReset() {
      if (initialized) {
        content.fillAmount = 0;
        currentValue = 0;
        maxValue = 0;

        adjustCoroutine = null;

        if (showTextValue && valueField != null) {
          valueField.text = string.Empty;
        }
      }
    }
    #endregion

  }

}
