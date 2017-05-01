using UnityEngine;

namespace Resource.Pooling {

    public abstract class Pool {
		protected GameObject container;
		protected bool initialized = false;

        public abstract void Return(PoolableObject aObject);
	}

}
