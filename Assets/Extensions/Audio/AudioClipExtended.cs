using UnityEngine;

namespace Extensions {

  [System.Serializable]
  public class AudioClipExtended {
    [SerializeField]
    private string id = null;
    [SerializeField]
    private AudioClip clip = null;
    [SerializeField, Range(0.0f, 1.0f)]
    private float volume = 1.0f;

    #region Getters & Setters
    public string ID {
      get { return id; }
    }

    public AudioClip Clip {
      get { return clip; }
    }

    public float Volume {
      get { return volume; }
    }
    #endregion

  }

}
