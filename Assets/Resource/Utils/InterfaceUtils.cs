using UnityEngine;
using UnityEngine.UI;

namespace Resource.Utils {

    public static class InterfaceUtils {

        #region Scroll Rect Management
        /// <summary>
        /// Snap the scroll rect to its target's location
        /// </summary>
        /// <param name="aScrollRect">Scroll Rect to move</param>
        /// <param name="aTarget">Target to snap to</param>
        public static void SnapTo(this ScrollRect aScrollRect, RectTransform aTarget) {
            Canvas.ForceUpdateCanvases();
            aScrollRect.content.anchoredPosition = (Vector2) aScrollRect.transform.InverseTransformPoint(aScrollRect.content.position) - (Vector2) aScrollRect.transform.InverseTransformPoint(aTarget.position);
            aScrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(aScrollRect.normalizedPosition.x), Mathf.Clamp01(aScrollRect.normalizedPosition.y));
        }

        /// <summary>
        /// Snap the scroll rect to its target's location to the position at the given time
        /// </summary>
        /// <param name="aScrollRect">Scroll Rect to move</param>
        /// <param name="aTarget">Target to snap to</param>
        /// <param name="aStartingPosition">Starting position to snap from</param>
        /// <param name="aTime">Time along the path to move to</param>
        public static void SnapTo(this ScrollRect aScrollRect, RectTransform aTarget, Vector2 aStartingPosition, float aTime) {
            Canvas.ForceUpdateCanvases();

            Vector2 targetPosition = (Vector2) aScrollRect.transform.InverseTransformPoint(aScrollRect.content.position) - (Vector2) aScrollRect.transform.InverseTransformPoint(aTarget.position);

            aScrollRect.content.anchoredPosition = Vector2.Lerp(aStartingPosition, targetPosition, aTime);
            aScrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(aScrollRect.normalizedPosition.x), Mathf.Clamp01(aScrollRect.normalizedPosition.y));
        }
        #endregion

    }

}
