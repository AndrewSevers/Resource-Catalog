using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions {

    public class AudioManager : Singleton<AudioManager> {
        [SerializeField]
        private AudioClip[] tracks;

        private Dictionary<string, AudioClip> clips;
        private Dictionary<string, AudioSource> sources;

        private bool initialized = false;
        private float musicVolume = 1;
        private float soundEffectsVolume = 1;

        #region Consts
        public const string BackgroundMusicID = "BackgroundMusic";
        public const string DefaultSoundEffectID = "DefaultSFX";
        #endregion

        #region Getters & Setters
        /// <summary>Music Volume from (0.0 - 1.0)</summary>
        public float MusicVolume {
            get { return musicVolume; }
            set {
                musicVolume = value;
                sources[BackgroundMusicID].volume = value;
            }
        }

        /// <summary>Sound Effects Volume from (0.0 - 1.0)</summary>
        public float SoundEffectsVolume {
            get { return soundEffectsVolume; }
            set {
                soundEffectsVolume = value;
                sources[DefaultSoundEffectID].volume = value;
            }
        }
        #endregion

        #region Initialization
        public void Initialize(float aMusicVolume = 1.0f, float aSoundEffectsVolume = 1.0f) {
            if (initialized == false) {
                clips = new Dictionary<string, AudioClip>();
                for (int i = 0; i < tracks.Length; i++) {
                    clips[tracks[i].name] = tracks[i];
                }

                sources = new Dictionary<string, AudioSource>();
                sources[BackgroundMusicID] = gameObject.AddComponent<AudioSource>();
                sources[BackgroundMusicID].loop = true;

                sources[DefaultSoundEffectID] = gameObject.AddComponent<AudioSource>();

                MusicVolume = aMusicVolume;
                SoundEffectsVolume = aSoundEffectsVolume;
            }

            initialized = true;
        }
        #endregion

        #region Audio Source Management
        /// <summary>
        /// Load an audio source based on the provided ID
        /// </summary>
        /// <param name="aSourceID">ID of the source to load</param>
        /// <param name="aCreate">If true an audio source with ID will be created</param>
        public AudioSource LoadAudioSource(string aSourceID, bool aCreate = false) {
            AudioSource source = null;

            if (sources.TryGetValue(aSourceID, out source) == false) {
                if (aCreate) {
                    source = gameObject.AddComponent<AudioSource>();
                    sources[aSourceID] = source;
                }
            }

            return source;
        }
        #endregion

        #region Music Management
        public void PlayMusic(string aClipID, string aAudioSourceID = BackgroundMusicID) {
            AudioClip clip = null;
            if (clips.TryGetValue(aClipID, out clip)) {
                PlayMusic(clip, aAudioSourceID);
            } else {
                Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available tracks!");
            }
        }

        public void PlayMusic(AudioClip aClip, string aAudioSourceID = BackgroundMusicID) {
            AudioSource source = sources[aAudioSourceID];

            // Play previously loaded track
            if (source.clip != null && source.clip.name == aClip.name) {
                if (source.isPlaying == false) {
                    source.Play();
                }

                return;
            }

            source.volume = musicVolume;
            source.clip = aClip;
            source.Play();
        }
        #endregion

        #region Sound Effects Management
        /// <summary>
        /// Play a one shot SFX through the default SFX audio source on the AudioManager
        /// </summary>
        /// <param name="aClipID">ID of the clip to play</param>
        /// <param name="aVolume">Volume of the clip</param>
        public void PlaySoundEffectOneShot(string aClipID, float aVolume = -1.0f) {
            AudioClip clip = null;
            if (clips.TryGetValue(aClipID, out clip)) {
                PlaySoundEffectOneShot(clip, aVolume);
            } else {
                Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available tracks!");
            }
        }

        /// <summary>
        /// Play a one shot SFX through the default SFX audio source on the AudioManager
        /// </summary>
        /// <param name="aClip">Clip to play</param>
        /// <param name="aVolume">Volume of the clip</param>
        public void PlaySoundEffectOneShot(AudioClip aClip, float aVolume = -1.0f) {
            if (aClip == null || soundEffectsVolume == 0) {
                return;
            }

            if (aVolume >= 0) {
                sources[DefaultSoundEffectID].PlayOneShot(aClip, aVolume * soundEffectsVolume);
            } else {
                sources[DefaultSoundEffectID].PlayOneShot(aClip, soundEffectsVolume);
            }
        }

        /// <summary>
        /// Play SFX through an audio source attached to the AudioManager
        /// </summary>
        /// <param name="aSourceID">ID of the audio source to use</param>
        /// <param name="aClipID">ID of the clip to play</param>
        /// <param name="aVolume">Volume of the clip</param>
        /// <param name="aLoop">If true the clip will loop</param>
        /// <param name="aCreateSource">If true and no audio source exists for the provided Source ID then a new audio source will be created</param>
        public void PlaySoundEffect(string aSourceID, string aClipID, float aVolume = -1.0f, bool aLoop = false, bool aCreateSource = false) {
            AudioSource source = LoadAudioSource(aSourceID, aCreateSource);
            if (source != null) {
                PlaySoundEffect(source, aClipID, aVolume, aLoop);
            } else {
                Debug.LogWarning("[AudioManager] No source by id \"" + aSourceID + "\" was found among the manager's list of available audio sources!");
            }
        }

        /// <summary>
        /// Play SFX through provided audio source
        /// </summary>
        /// <param name="aSource">Audio source to use</param>
        /// <param name="aClipID">ID of the clip to play</param>
        /// <param name="aVolume">Volume of the clip</param>
        /// /// <param name="aLoop">If true the clip will loop</param>
        public void PlaySoundEffect(AudioSource aSource, string aClipID, float aVolume = -1.0f, bool aLoop = false) {
            AudioClip clip = null;
            if (clips.TryGetValue(aClipID, out clip)) {
                PlaySoundEffect(aSource, clip, aVolume, aLoop);
            } else {
                Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available tracks!");
            }
        }

        /// <summary>
        /// Play SFX through provided audio source
        /// </summary>
        /// <param name="aSource">Audio source to use</param>
        /// <param name="aCLip">Clip to play</param>
        /// <param name="aVolume">Volume of the clip</param>
        /// <param name="aLoop">If true the clip will loop</param>
        public void PlaySoundEffect(AudioSource aSource, AudioClip aClip = null, float aVolume = -1.0f, bool aLoop = false) {
            aSource.loop = aLoop;

            if (aClip != null) {
                aSource.clip = aClip;
            }

            if (aVolume >= 0) {
                aSource.volume = aVolume * soundEffectsVolume;
            } else {
                aSource.volume = soundEffectsVolume;
            }

            aSource.Play();
        }
        #endregion

        #region State Utilities
        /// <summary>
        /// Stop the audio source
        /// </summary>
        /// <param name="aSourceID">ID of the audio source to stop</param>
        public void Stop(string aSourceID) {
            AudioSource source = LoadAudioSource(aSourceID);
            if (source != null) {
                source.Stop();
            }
        }

        /// <summary>
        /// Stop the audio source
        /// </summary>
        /// <param name="aSourceID">Audio source to stop</param>
        public void Stop(AudioSource aSource) {
            aSource.Stop();
        }

        /// <summary>
        /// Pause or unpause the audio source
        /// </summary>
        /// <param name="aPause">Whether to pause or unpause the audio source</param>
        /// <param name="aSourceID">ID of the audio source to stop</param>
        public void Pause(bool aPause, string aSourceID) {
            AudioSource source = LoadAudioSource(aSourceID);
            if (source != null) {
                Pause(aPause, source);
            }
        }

        /// <summary>
        /// Pause or unpause the audio source
        /// </summary>
        /// <param name="aPause">Whether to pause or unpause the audio source</param>
        /// <param name="aSourceID">ID of the audio source to stop</param>
        public void Pause(bool aPause, AudioSource aSource) {
            if (aPause) {
                aSource.Pause();
            } else {
                aSource.UnPause();
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Fade in new music whlie fading out current music
        /// </summary>
        public void Crossfade() {
            // TODO: FILL IN
        }

        /// <summary>
        /// Fade in the provided audio source (by ID)
        /// </summary>
        /// <param name="aSourceID">ID for the audio source</param>
        /// <param name="aFadeDuration">Duration of the fade</param>
        public void FadeIn(string aSourceID, float aFadeDuration = 1.0f) {
            StartCoroutine(ProcessFade(aSourceID, 1.0f, aFadeDuration));
        }

        /// <summary>
        /// Fade out the provided audio source (by ID)
        /// </summary>
        /// <param name="aSourceID">ID for the audio source</param>
        /// <param name="aFadeDuration">Duration of the fade</param>
        public void FadeOut(string aSourceID, float aFadeDuration = 1.0f) {
            StartCoroutine(ProcessFade(aSourceID, 0.0f, aFadeDuration));
        }

        private IEnumerator ProcessFade(string aSourceID, float aFinal, float aDuration) {
            float time = 0.0f;

            AudioSource source;
            if (sources.TryGetValue(aSourceID, out source)) {
                float start = source.volume;

                do {
                    time += (Time.deltaTime / aDuration);

                    source.volume = Mathf.Lerp(start, aFinal, time);

                    yield return null;
                } while (time < 1.0f);
            }
        }

        /// <summary>
        /// Mute/Un-mute audio sources, if no source ID is provided all sources will be muted
        /// </summary>
        /// <param name="aMute">Whether to mute or unmute</param>
        /// <param name="aSourceID">Source to mute. If left blank all sources will be muted</param>
        public void Mute(bool aMute, string aSourceID = null) {
            if (string.IsNullOrEmpty(aSourceID)) {
                foreach (AudioSource source in sources.Values) {
                    source.mute = aMute;
                }
            } else {
                AudioSource source;
                if (sources.TryGetValue(aSourceID, out source)) {
                    source.mute = aMute;
                }
            }
        }
        #endregion

    }

}
