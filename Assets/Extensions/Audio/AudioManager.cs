using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions {

  public class AudioManager : Singleton<AudioManager> {
    [SerializeField, Tooltip("_tracks to load at start of application")]
    private AudioClip[] _tracks = null;

    private Dictionary<string, AudioClip> _clips = null;
    private Dictionary<string, AudioSource> _sources = null;

    private bool _initialized = false;
    private float musicVolume = 1;
    private float soundEffectsVolume = 1;

    #region Consts
    public const string DefaultMusicSourceID = "DefaultBackgroundMusic";
    public const string DefaultSoundEffectSourceID = "DefaultSoundEffect";
    #endregion

    #region Getters & Setters
    /// <summary>Music Volume from (0.0 - 1.0)</summary>
    public float MusicVolume {
      get { return musicVolume; }
      set {
        musicVolume = Mathf.Clamp01(value);
        _sources[DefaultMusicSourceID].volume = musicVolume;
      }
    }

    /// <summary>Sound Effects Volume from (0.0 - 1.0)</summary>
    public float SoundEffectsVolume {
      get { return soundEffectsVolume; }
      set {
        soundEffectsVolume = Mathf.Clamp01(value);
        _sources[DefaultSoundEffectSourceID].volume = soundEffectsVolume;
      }
    }
    #endregion

    #region Initialization
    public void Initialize(float aMusicVolume = 1.0f, float aSoundEffectsVolume = 1.0f) {
      if (_initialized == false) {
        _clips = new Dictionary<string, AudioClip>();
        for (int i = 0; i < _tracks.Length; i++) {
          _clips[_tracks[i].name] = _tracks[i];
        }

        _sources = new Dictionary<string, AudioSource>();
        _sources[DefaultMusicSourceID] = gameObject.AddComponent<AudioSource>();
        _sources[DefaultMusicSourceID].loop = true;

        _sources[DefaultSoundEffectSourceID] = gameObject.AddComponent<AudioSource>();

        MusicVolume = aMusicVolume;
        SoundEffectsVolume = aSoundEffectsVolume;
      }

      _initialized = true;
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

      if (_sources.TryGetValue(aSourceID, out source) == false) {
        if (aCreate) {
          source = gameObject.AddComponent<AudioSource>();
          _sources[aSourceID] = source;
        }
      }

      return source;
    }

    /// <summary>
    /// Load an audio source based on the provided ID
    /// </summary>
    /// <param name="aSourceID">ID of the source to load</param>
    /// <param name="aLoadedSource">Loaded audio source</param>
    /// <param name="aCreate">If true an audio source with ID will be created</param>
    /// <returns>Returns true if the audio source was loaded</returns>
    public bool LoadAudioSource(string aSourceID, out AudioSource aLoadedSource, bool aCreate = false) {
      aLoadedSource = null;

      if (_sources.TryGetValue(aSourceID, out aLoadedSource) == false) {
        if (aCreate) {
          aLoadedSource = gameObject.AddComponent<AudioSource>();
          _sources[aSourceID] = aLoadedSource;
        } else {
          return false;
        }
      }

      return true;
    }
    #endregion

    #region Music Management
    public void PlayMusic(string aClipID, string aSourceID = DefaultMusicSourceID, bool aCreateSource = false) {
      AudioClip clip = null;
      if (_clips.TryGetValue(aClipID, out clip)) {
        PlayMusic(clip, aSourceID, aCreateSource);
      } else {
        Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available _tracks!");
      }
    }

    public void PlayMusic(AudioClip aClip, string aSourceID = DefaultMusicSourceID, bool aCreateSource = false) {
      AudioSource source = LoadAudioSource(aSourceID, aCreateSource);

      // Play previously loaded track
      if (source != null) {
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
      if (_clips.TryGetValue(aClipID, out clip)) {
        PlaySoundEffectOneShot(clip, aVolume);
      } else {
        Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available _tracks!");
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
        _sources[DefaultSoundEffectSourceID].PlayOneShot(aClip, aVolume * soundEffectsVolume);
      } else {
        _sources[DefaultSoundEffectSourceID].PlayOneShot(aClip, soundEffectsVolume);
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
        Debug.LogWarning("[AudioManager] No source by id \"" + aSourceID + "\" was found among the manager's list of available audio _sources!");
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
      if (_clips.TryGetValue(aClipID, out clip)) {
        PlaySoundEffect(aSource, clip, aVolume, aLoop);
      } else {
        Debug.LogWarning("[AudioManager] No clip by id \"" + aClipID + "\" was found among the manager's list of available _tracks!");
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

    #region Fades
    /// <summary>
    /// Fade in new music whlie fading out current music
    /// </summary>
    /// <param name="aOutgoingSourceID">ID for the audio source to fade out</param>
    /// <param name="aIncomingSourceID">ID for the audio source to fade in</param>
    /// <param name="aDuration">Duration of the fade</param>
    /// <param name="aCreate_sources">Enforce creation of audio _sources if they don't exist</param>
    public void Crossfade(string aOutgoingSourceID, string aIncomingSourceID, float aDuration, bool aCreate_sources = false) {
      Crossfade(LoadAudioSource(aOutgoingSourceID, aCreate_sources), LoadAudioSource(aIncomingSourceID, aCreate_sources), aDuration);
    }

    /// <summary>
    /// Fade in new music whlie fading out current music
    /// </summary>
    /// <param name="aOutgoingSource">Audio source to fade out</param>
    /// <param name="aIncomingSource">Audio source to fade in</param>
    /// <param name="aDuration">Duration of the fade</param>
    public void Crossfade(AudioSource aOutgoingSource, AudioSource aIncomingSource, float aDuration) {
      FadeOut(aOutgoingSource, aDuration);
      FadeIn(aIncomingSource, aDuration);
    }

    /// <summary>
    /// Fade in the provided audio source (by ID)
    /// </summary>
    /// <param name="aSourceID">ID for the audio source</param>
    /// <param name="aDuration">Duration of the fade</param>
    public void FadeIn(string aSourceID, float aDuration = 1.0f) {
      FadeIn(LoadAudioSource(aSourceID), aDuration);
    }

    /// <summary>
    /// Fade in the provided audio source (by ID)
    /// </summary>
    /// <param name="aSource">Audio source to fade in</param>
    /// <param name="aDuration">Duration of the fade</param>
    public void FadeIn(AudioSource aSource, float aDuration = 1.0f) {
      StartCoroutine(ProcessFade(aSource, 1.0f, aDuration));
    }

    /// <summary>
    /// Fade out the provided audio source (by ID)
    /// </summary>
    /// <param name="aSourceID">ID for the audio source</param>
    /// <param name="aDuration">Duration of the fade</param>
    public void FadeOut(string aSourceID, float aDuration = 1.0f) {
      FadeOut(LoadAudioSource(aSourceID), aDuration);
    }

    /// <summary>
    /// Fade out the provided audio source (by ID)
    /// </summary>
    /// <param name="aSource">Audio source to fade out</param>
    /// <param name="aDuration">Duration of the fade</param>
    public void FadeOut(AudioSource aSource, float aDuration = 1.0f) {
      StartCoroutine(ProcessFade(aSource, 0.0f, aDuration));
    }

    private IEnumerator ProcessFade(AudioSource aSource, float aFinal, float aDuration) {
      if (aSource != null) {
        float time = 0.0f;

        // Ensure the source is playing before fading in/out
        if (aSource.isPlaying == false) {
          aSource.Play();
        }

        float start = aSource.volume;

        do {
          time += (Time.deltaTime / aDuration);

          aSource.volume = Mathf.Lerp(start, aFinal, time);

          yield return null;
        } while (time < 1.0f);
      }
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Mute/Un-mute audio _sources, if no source ID is provided all _sources will be muted
    /// </summary>
    /// <param name="aMute">Whether to mute or unmute</param>
    /// <param name="aSourceID">Source to mute. If left blank all _sources will be muted</param>
    public void Mute(bool aMute, string aSourceID = null) {
      if (string.IsNullOrEmpty(aSourceID)) {
        foreach (AudioSource source in _sources.Values) {
          source.mute = aMute;
        }
      } else {
        AudioSource source;
        if (_sources.TryGetValue(aSourceID, out source)) {
          source.mute = aMute;
        }
      }
    }
    #endregion

  }

}
