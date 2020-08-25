using Extensions;
using UnityEngine;

namespace Extensions {

  public class AudioElement : MonoBehaviour {
    [SerializeField]
    private AudioClipExtended[] _clips = null;

    #region Music
    public void PlayMusic(string clipId) {
      AudioClipExtended clip = System.Array.Find(_clips, c => c.ID == clipId);
      if (clip != null) {
        PlayMusic(clip);
      } else {
        AudioManager.Instance.PlayMusic(clipId);
      }
    }

    public void PlayMusic(AudioClip clip) {
      AudioManager.Instance.PlayMusic(clip);
    }

    public void PlayMusic(AudioClipExtended clip) {
      AudioManager.Instance.PlayMusic(clip.Clip);
    }

    public void PlayMusic(AnimationEvent animationEvent) {
      AudioManager.Instance.PlayMusic(animationEvent.stringParameter);
    }
    #endregion

    #region SFX
    public void PlaySFX(string clipId) {
      AudioClipExtended clip = System.Array.Find(_clips, c => c.ID == clipId);
      if (clip != null) {
        PlaySFX(clip);
      } else {
        AudioManager.Instance.PlaySoundEffectOneShot(clipId);
      }
    }

    public void PlaySFX(string clipId, float volume) {
      AudioManager.Instance.PlaySoundEffectOneShot(clipId, volume);
    }

    public void PlaySFX(AudioClip clip) {
      AudioManager.Instance.PlaySoundEffectOneShot(clip);
    }

    public void PlaySFX(AudioClipExtended clip) {
      AudioManager.Instance.PlaySoundEffectOneShot(clip.Clip, clip.Volume);
    }

    public void PlaySFXByAnimation(AnimationEvent animationEvent) {
      AudioManager.Instance.PlaySoundEffectOneShot(animationEvent.stringParameter, animationEvent.floatParameter);
    }
    #endregion
  }

}
