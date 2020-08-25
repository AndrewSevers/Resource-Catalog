using UnityEngine;

namespace Extensions {

  [System.Serializable]
  public class ActionEvent {
    [SerializeField, Tooltip("ID of the action")]
    private string id = null;
    [SerializeField, Tooltip("Delay before the action is executed")]
    private float delay = 0;
    [SerializeField, Tooltip("Id of the action's variant")]
    private int variantId = 0;

    #region Getters & Setters
    public string ID {
      get { return id; }
    }

    public float Delay {
      get { return delay; }
    }

    public int VariantId {
      get { return variantId; }
    }
    #endregion

  }

}
