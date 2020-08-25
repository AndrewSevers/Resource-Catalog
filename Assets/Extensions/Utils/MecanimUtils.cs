using UnityEngine;

namespace Extensions.Utils {

  public static class MecanimUtils {

    /// <summary>
    /// Set the trigger for the animator if that animation is currently not running (determined by tag)
    /// </summary>
    /// <param name="aAnimator">Animator to use</param>
    /// <param name="aTrigger">Trigger to set</param>
    /// <param name="aTag">Tag to compare against. If this tag is set and the current state matches it the trigger will not be set.</param>
    public static bool SetTrigger(this Animator aAnimator, string aTrigger, string aTag) {
      if (string.IsNullOrEmpty(aTag) || aAnimator.GetCurrentAnimatorStateInfo(0).IsTag(aTag) == false) {
        aAnimator.SetTrigger(aTrigger);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Restart the animation state if that animation state is playing
    /// </summary>
    /// <param name="aAnimator">Animator to use</param>
    /// <param name="aStateName">State to restart</param>
    /// <param name="aLayer">Layer the state is on</param>
    /// <returns>Whether ot not the animation was succesfully restarted</returns>
    public static bool Restart(this Animator aAnimator, string aStateName, int aLayer = 0) {
      if (aAnimator.GetCurrentAnimatorStateInfo(aLayer).IsName(aStateName)) {
        aAnimator.Play(aStateName, aLayer, 0);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Start an animation at a specific time (or random time if no value is provided)
    /// </summary>
    /// <param name="aAnimator">Animator to use</param>
    /// <param name="aStateName">State to play</param>
    /// <param name="aLayer">Layer the state is on</param>
    /// <param name="aTime">The time to start the animation (if left empty the time will be random)</param>
    public static void PlayWithStartTime(this Animator aAnimator, string aStateName, int aLayer = 0, float aTime = -1) {
      if (aTime == -1) {
        aTime = Random.value;
      }

      aAnimator.Play(aStateName, aLayer, aTime);
    }

  }

}
