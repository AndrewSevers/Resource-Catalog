using Resource.Properties;
using System.Collections;
using UnityEngine;

namespace Resource.UI {

    /// <summary>
    /// UI Element that can fade in and out (including all of its children)
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
	public class FadeElement : MonoBehaviour {
		[SerializeField, Min(0.01f), Tooltip("Duration it takes to fade in/out. Minimum value = 0.01")]
		private float duration = 1.0f;

		private CanvasGroup canvasGroup;
		private Coroutine fadeCoroutine;
		private CachedFade existingFade;

		#region PausedFade Struct
		private class CachedFade {
			private float start;
			private float end;
            private float rate;
			private System.Action callback;

			#region Getters & Setters
			public float Start {
				get { return start; }
				set { start = value; }
			}

			public float End {
				get { return end; }
			}

            public float Rate {
                get { return rate; }
            }

            public System.Action Callback {
				get { return callback; }
			}
			#endregion

			#region Constructors
			public CachedFade(float aStart, float aEnd, float aRate, System.Action aCallback) {
				start = aStart;
				end = aEnd;
                rate = aRate;
				callback = aCallback;
			}
			#endregion

			#region Utility Functions
			/// <summary>
			/// Update all the values of the processed fade
			/// </summary>
			public void Update(float aStart, float aEnd, float aRate, System.Action aCallback) {
				start = aStart;
				end = aEnd;
                rate = aRate;
				callback = aCallback;
			}
			#endregion

		}
		#endregion

		#region Initialization
		private void Awake() {
			canvasGroup = GetComponent<CanvasGroup>();
		}
        #endregion

        #region Fade In
        /// <summary>
        /// Fade the interface element out (invisible -> visible)
        /// </summary>
        public void FadeIn(float aRate = -1, System.Action aCompletionCallback = null) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			fadeCoroutine = StartCoroutine(ProcessFade(0.0f, 1.0f, aRate, aCompletionCallback));
		}

        /// <summary>
        /// Fade the interface element out (invisible -> visible)
        /// </summary>
        public IEnumerator FadeInAsync(float aRate = -1, System.Action aCompletionCallback = null) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			yield return StartCoroutine(ProcessFade(0.0f, 1.0f, aRate, aCompletionCallback));
		}
		#endregion

		#region Fade Out
		/// <summary>
		/// Fade the interface element out (visible -> invisible)
		/// </summary>
		public void FadeOut(float aRate = -1, System.Action aCompletionCallback = null) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			fadeCoroutine = StartCoroutine(ProcessFade(1.0f, 0.0f, aRate, aCompletionCallback));
		}

		/// <summary>
		/// Fade the interface element out (visible -> invisible)
		/// </summary>
		public IEnumerator FadeOutAsync(float aRate = -1, System.Action aCompletionCallback = null) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			yield return StartCoroutine(ProcessFade(1.0f, 0.0f, aRate, aCompletionCallback));
		}
		#endregion

		#region Fade Manually
		/// <summary>
		/// Fade the interface element out from the given start to the given end
		/// </summary>
		public void Fade(float aStart, float aEnd, float aRate = -1, System.Action aCompletionCallback = null) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			// Ensure the start and end values are within an acceptable range
			aStart = Mathf.Clamp(aStart, 0.0f, 1.0f);
			aEnd = Mathf.Clamp(aEnd, 0.0f, 1.0f);

			fadeCoroutine = StartCoroutine(ProcessFade(aStart, aEnd, aRate, aCompletionCallback));
		}

		/// <summary>
		/// Fade the interface element out from the given start to the given end
		/// </summary>
		public IEnumerator FadeAsync(float aStart, float aEnd, float aRate = -1) {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			}

			// Ensure the object is actually active before fading
			if (gameObject.activeSelf == false) {
				gameObject.SetActive(true);
			}

			// Ensure the start and end values are within an acceptable range
			aStart = Mathf.Clamp(aStart, 0.0f, 1.0f);
			aEnd = Mathf.Clamp(aEnd, 0.0f, 1.0f);

			yield return StartCoroutine(ProcessFade(aStart, aEnd, aRate, null));
		}
		#endregion

		#region Fade Utilities
		/// <summary>
		/// Restart the fade from its previously paused status
		/// </summary>
		public void ResumeFade() {
			if (fadeCoroutine == null && existingFade != null) {
				fadeCoroutine = StartCoroutine(ProcessFade(existingFade.Start, existingFade.End, existingFade.Rate, existingFade.Callback));
			} else {
				Debug.LogWarning(string.Format("GameObject '{0}' does not have have any stored values to restart from. Only call this from a paused state!", gameObject));
			}
		}

		/// <summary>
		/// Pause the object's fade and store its current status
		/// </summary>
		public void PauseFade() {
			if (fadeCoroutine != null) {
				existingFade.Start = canvasGroup.alpha;
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			} else {
				Debug.LogWarning(string.Format("GameObject '{0}' is currently not fading. Start fading this element prior to making this call!", gameObject));
			}
		}

		/// <summary>
		/// Stop the object's fade completely (cannot resume after stopping)
		/// </summary>
		public void StopFade() {
			if (fadeCoroutine != null) {
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			} else {
				Debug.LogWarning(string.Format("GameObject '{0}' is currently not fading. Start fading this element prior to making this call!", gameObject));
			}
		}
		#endregion

		#region Fade Processing
		private IEnumerator ProcessFade(float aStart, float aEnd, float aRate, System.Action aCallback = null) {
			existingFade = new CachedFade(aStart, aEnd, aRate, aCallback);

            if (aRate == -1) {
                aRate = duration;
            }

			float currentTime = 0;
			canvasGroup.alpha = aStart;

			canvasGroup.blocksRaycasts = true;

			// Lerp the alpha of the Fade Element
			do {
				currentTime += (Time.deltaTime / duration);
				canvasGroup.alpha = Mathf.Lerp(aStart, aEnd, currentTime);
				yield return null;
			} while (canvasGroup.alpha != aEnd);

			// Ensure the fade is completely finished
			yield return null;

			fadeCoroutine = null;
			existingFade = null;

			canvasGroup.blocksRaycasts = (aEnd != 0);

			if (aCallback != null) {
				aCallback();
			}
		}
		#endregion

	}

}
