using Resource.Properties;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Resource.UI {

    [RequireComponent(typeof(Image))]
    public class CooldownElement : MonoBehaviour {
        [SerializeField]
        private float maxValue;

        [Header("Text Element")]
        [SerializeField]
        private bool showTextValue;
        [SerializeField, Visibility("showTextValue", true)]
        private bool showAsPercentage;
        [SerializeField, Visibility("showTextValue", true)]
        private Text valueField;
        [SerializeField, Visibility("showTextValue", true)]
        private string valueFormat;

        private Image content;
        private float currentValue;
        private bool initialized;

        public delegate void OnCooldownCompleteEvent();
        public OnCooldownCompleteEvent OnCooldownComplete;

        #region Getters & Setters
        public bool IsOnCooldown {
            get { return currentValue != 0; }
        }
        #endregion

        #region Initialization
        public void Initialize(float aMaxValue) {
            if (initialized == false) {
                content = GetComponent<Image>();
            }

            maxValue = aMaxValue;

            initialized = true;
        }
        #endregion

        #region Cooldown Management
        public void StartCooldown() {
            if (showTextValue && valueField != null) {
                valueField.enabled = true;
            }

            StartCoroutine(ProcessCooldown());
        }

        private IEnumerator ProcessCooldown() {
            currentValue = maxValue;

            while (currentValue > 0.5f) {
                currentValue -= Time.deltaTime;
                content.fillAmount = (currentValue / maxValue);

                if (showTextValue && valueField != null) {
                    valueField.text = string.Format(valueFormat, Mathf.RoundToInt(currentValue));
                }

                yield return null;
            }

            currentValue = 0;
            content.fillAmount = 0;

            if (showTextValue && valueField != null) {
                valueField.enabled = false;
            }

            if (OnCooldownComplete != null) {
                OnCooldownComplete();
            }
        }

        public void ClearCooldown() {
            if (content != null) {
                content.fillAmount = 0;
                currentValue = 0;

                if (showTextValue && valueField != null) {
                    valueField.enabled = false;
                }
            }
        }
        #endregion

        #region Cleanup
        public void Reset() {
            ClearCooldown();
        }
        #endregion

    }

}
