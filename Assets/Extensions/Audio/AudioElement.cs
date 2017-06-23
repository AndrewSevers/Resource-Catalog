using Extensions;
using UnityEngine;

namespace Extensions {

    public class AudioElement : MonoBehaviour {
        [SerializeField]
        private AudioClipExtended[] clips;

        #region Music
        public void PlayMusic(string aClipID) {
            AudioClipExtended clip = System.Array.Find(clips, c => c.ID == aClipID);
            if (clip != null) {
                PlayMusic(clip);
            } else {
                AudioManager.Instance.PlayMusic(aClipID);
            }
        }

        public void PlayMusic(AudioClip aAudioClip) {
            AudioManager.Instance.PlayMusic(aAudioClip);
        }

        public void PlayMusic(AudioClipExtended aAudioClip) {
            AudioManager.Instance.PlayMusic(aAudioClip.Clip);
        }

        public void PlayMusic(AnimationEvent aEvent) {
            AudioManager.Instance.PlayMusic(aEvent.stringParameter);
        }
        #endregion

        #region SFX
        public void PlaySFX(string aClipID) {
            AudioClipExtended clip = System.Array.Find(clips, c => c.ID == aClipID);
            if (clip != null) {
                PlaySFX(clip);
            } else {
                AudioManager.Instance.PlaySoundEffectOneShot(aClipID);
            }
        }

        public void PlaySFX(string aClipID, float aVolume) {
            AudioManager.Instance.PlaySoundEffectOneShot(aClipID, aVolume);
        }

        public void PlaySFX(AudioClip aAudioClip) {
            AudioManager.Instance.PlaySoundEffectOneShot(aAudioClip);
        }

        public void PlaySFX(AudioClipExtended aAudioClip) {
            AudioManager.Instance.PlaySoundEffectOneShot(aAudioClip.Clip, aAudioClip.Volume);
        }

        public void PlaySFXByAnimation(AnimationEvent aEvent) {
            AudioManager.Instance.PlaySoundEffectOneShot(aEvent.stringParameter, aEvent.floatParameter);
        }
        #endregion
    }

}
