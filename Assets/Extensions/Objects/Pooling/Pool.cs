using UnityEngine;

namespace Extensions.Pooling {

  public abstract class Pool {
    protected GameObject container = null;
    protected bool initialized = false;

    public abstract void Return(PoolableObject aObject);
  }

}
