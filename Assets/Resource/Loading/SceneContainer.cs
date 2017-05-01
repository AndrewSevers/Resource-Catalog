using UnityEngine;

namespace Resource {

	public class SceneContainer : MonoBehaviour {
		[SerializeField]
		private string sceneName;
		[SerializeField]
		private GameObject objectsContainer;
		[SerializeField]
		private bool requiresCompletionEvent = false;

		#region Getters & Setters
		public string SceneName {
			get { return sceneName; }
		}

		public GameObject ObjectsContainer {
			get { return objectsContainer; }
		}

		public bool RequiresCompletionEvent {
			get { return (requiresCompletionEvent && LoadingManager.Instance.SceneCount > 1); }
		}
		#endregion

		#region Initialization
		void Awake() {
            if (LoadingManager.Instance.SceneCount > 1) {
                if (objectsContainer.activeSelf) {
                    objectsContainer.SetActive(false);
                }
            }

            LoadingManager.Instance.AddScene(this);
		}
		#endregion

		#region Cleanup
		void OnDestroy() {
			LoadingManager gameInstance = LoadingManager.Instance;

			if (gameInstance != null) {
				gameInstance.RemoveScene(this);
			}
		}
		#endregion

	}

}
