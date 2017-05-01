using Resource.Properties;
using Resource.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resource {

    public class LoadingManager : Singleton<LoadingManager> {
		[SerializeField]
		private string loadingSceneName;
		[SerializeField, Tooltip("Whether or not to keep the loading scene around after loading is complete")]
		private bool retainLoadingScene = true;
        [SerializeField, Range(0, 5), Tooltip("Minimum amount of time loading can take.\n\nIncrease this to reduce the effect of a 'blinking' loading screen.")]
        private float minLoadingTime = 1.0f;

		[Header("Animation")]
		[SerializeField, Tooltip("Either use animator for transition or use a fade element")]
		private bool useAnimation;
		[SerializeField, Visibility("useAnimation", true), Tooltip("Animator with the following triggers [\"Fade In, Fade Out\"")]
		private Animator animator;
		[SerializeField, Visibility("useAnimation", false)]
		private FadeElement fadeElement;

		private Dictionary<string, LoadingOperation> loadingOperations = new Dictionary<string, LoadingOperation>();
		private List<SceneContainer> scenes = new List<SceneContainer>();
		private Action loadingPreFadeOutCallback;
		private Action loadingCompletionCallback;
		private bool sceneLoadingCompleted = false;
		private bool loading;

		#region Getters & Setters
		/// <summary>Alias for SceneManager.GetActiveScene()</summary>
		public Scene ActiveScene {
			get { return SceneManager.GetActiveScene(); }
		}

		/// <summary>Alias for SceneManager.sceneCount</summary>
		public int SceneCount {
			get { return SceneManager.sceneCount; }
		}

		public bool IsLoading {
			get { return loading; }
		}
		#endregion

		#region Scene Management
		public Scene FindScene(string aSceneName) {
			return SceneManager.GetSceneByName(aSceneName);
		}

		public SceneContainer FindSceneContainer(string aSceneName) {
			return scenes.Find(s => s.SceneName == aSceneName);
		}

		public void AddScene(SceneContainer aScene) {
			scenes.Add(aScene);
		}

		public void RemoveScene(SceneContainer aScene) {
			scenes.Remove(aScene);
		}
		#endregion

		#region Loading
		/// <summary>Preload the given level</summary>
		/// <param name="aSceneName">Scene to load</param>
		/// <param name="aPostFadeAction">Action to perform after the initial fade</param>
		public void PreloadScene(string aSceneName) {
			if (FindSceneContainer(aSceneName) == null) {
				loadingOperations[aSceneName] = new LoadingOperation(SceneManager.LoadSceneAsync(aSceneName, LoadSceneMode.Additive));
			}
		}

		/// <summary>Load the given level</summary>
		/// <param name="aSceneName">Scene to load</param>
		public void LoadScene(string aSceneName, params LoadingData[] aLoadingData) {
			loading = true;

            // Setup loading data if provided
			LoadingOperation loadingData = null;
			if (loadingOperations.TryGetValue(aSceneName, out loadingData) == false) {
				loadingData = new LoadingOperation();
				loadingOperations[aSceneName] = loadingData;
			}

			if (aLoadingData != null) {
				loadingData.AppendData(aLoadingData);
			}

			// Ensure loading can progress if the game is completely paused
			if (Time.timeScale == 0) {
				Time.timeScale = 1;
			}

            // Reloading scene clear previous operation
            if (ActiveScene.name == aSceneName) {
                loadingData.Operation = null;
            }

			StartCoroutine(ProcessLoadingScene(aSceneName, loadingData));
		}

		private IEnumerator ProcessLoadingScene(string aSceneName, LoadingOperation aLoadingData) {
			yield return WaitForTransition(true);

			// Display loading scene
			SceneContainer loadingScene = FindSceneContainer(loadingSceneName);
			if (loadingScene == null) {
				yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
				loadingScene = FindSceneContainer(loadingSceneName);
			}

			loadingScene.ObjectsContainer.SetActive(true);

			yield return WaitForTransition(false);

            // Clear the active scenes loading operation unless we are reloading the same scene
            if (ActiveScene.name != aSceneName) {
                loadingOperations.Remove(ActiveScene.name);
            }

            // Unload current scene
            yield return SceneManager.UnloadSceneAsync(ActiveScene);

			// If we didn't preload the scene then load it now
			if (aLoadingData.Operation == null) {
				aLoadingData.Operation = SceneManager.LoadSceneAsync(aSceneName, LoadSceneMode.Additive);
			}

			float elapsedTime = Time.time;

			// Wait for the scene to complete loading
			while (aLoadingData.Operation.isDone == false) {
				yield return null;
			}

			SceneContainer scene = FindSceneContainer(aSceneName);

			// Unload all scenes that aren't the active loading scenes
			for (int i = SceneManager.sceneCount - 1; i >= 0; i--) {
				string activeSceneName = SceneManager.GetSceneAt(i).name;

				if (activeSceneName != aSceneName && activeSceneName != loadingSceneName) {
					yield return SceneManager.UnloadSceneAsync(activeSceneName);
					loadingOperations.Remove(activeSceneName);
				}
			}

            // Set the current scene to be the primary active one. This is required otherwise objects will be instantiated into the wrong scene.
            SceneManager.SetActiveScene(FindScene(aSceneName));
			scene.ObjectsContainer.SetActive(true);

			// Wait for a completion event if the scene requires one
			if (scene.RequiresCompletionEvent) {
				while (sceneLoadingCompleted == false) {
					yield return null;
				}
				sceneLoadingCompleted = false;
			}

			yield return WaitForTransition(true);
			yield return new WaitForEndOfFrame();

			// Unload/Disable loading scene
			if (retainLoadingScene) {
				FindSceneContainer(loadingSceneName).ObjectsContainer.SetActive(false);
			} else {
				yield return SceneManager.UnloadSceneAsync(loadingSceneName);
			}

            // Too keep memory low ensure unused assets are cleared upon loading
            yield return Resources.UnloadUnusedAssets();

			// Run an additional callback before the loading fades out into the active scene
			if (loadingPreFadeOutCallback != null) {
				loadingPreFadeOutCallback();
				loadingPreFadeOutCallback = null;
			}

			// Add an additional time offset if we don't want the loading screen to possibly blink
			elapsedTime = Time.time - elapsedTime;
			if (elapsedTime <= minLoadingTime) {
				yield return new WaitForSeconds(minLoadingTime - elapsedTime);
			}

			// Fade into the newly loaded scene
			yield return WaitForTransition(false);
			yield return new WaitForEndOfFrame();

			// Run a callback once the scene is in view
			if (loadingCompletionCallback != null) {
				loadingCompletionCallback();
				loadingCompletionCallback = null;
			}

			loading = false;
		}
		#endregion

		#region Utility Functions
		private IEnumerator WaitForTransition(bool aTransitionIn) {
			if (useAnimation) {
				if (aTransitionIn) {
					animator.SetTrigger("Fade In");
				} else {
					animator.SetTrigger("Fade Out");
				}

				do {
					yield return null;
				} while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
			} else {
				if (aTransitionIn) {
					yield return StartCoroutine(fadeElement.FadeInAsync());
				} else {
					yield return StartCoroutine(fadeElement.FadeOutAsync());
				}
			}
		}

		public bool LoadingOperationExists(string aSceneName) {
            return loadingOperations.ContainsKey(aSceneName);
		}

		public LoadingOperation GetLoadingOperation(string aKey) {
			LoadingOperation data = null;
			loadingOperations.TryGetValue(aKey, out data);

			return data;
		}

		public T GetLoadingData<T>(string aSceneName, LoadingDataType aKey) {
			LoadingOperation operation = null;
			loadingOperations.TryGetValue(aSceneName, out operation);

			object value = null;

			if (operation != null) {
				value = operation.GetDataValue<T>(aKey);
			}

			return (T) value;
		}

		public void TriggerCompleteEvent(Action aPreFadeOutCallback = null, Action aCompletionCallback = null) {
			sceneLoadingCompleted = true;
			loadingPreFadeOutCallback = aPreFadeOutCallback;
			loadingCompletionCallback = aCompletionCallback;
		}
		#endregion
	}

	#region Loading Operation Class
	public class LoadingOperation {
		private AsyncOperation operation = null;
		private Dictionary<LoadingDataType, object> data;

		#region Getters & setters
		public AsyncOperation Operation {
			get { return operation; }
			set { operation = value; }
		}
		#endregion

		#region Constructor
		public LoadingOperation() {
			data = new Dictionary<LoadingDataType, object>();
		}

		public LoadingOperation(Dictionary<LoadingDataType, object> aData) {
			data = (aData != null) ? aData : new Dictionary<LoadingDataType, object>();
		}

		public LoadingOperation(AsyncOperation aOperation, Dictionary<LoadingDataType, object> aData = null) {
			operation = aOperation;
			data = (aData != null) ? aData : new Dictionary<LoadingDataType, object>();
		}
		#endregion

		#region Utility Function
		public void AddData(LoadingData aData) {
			data[aData.Key] = aData.Value;
		}

		public void AppendData(params LoadingData[] aData) {
			foreach (LoadingData incomingData in aData) {
				data[incomingData.Key] = incomingData.Value;
			}
		}

		public T GetDataValue<T>(LoadingDataType aKey) {
			object value = null;
			data.TryGetValue(aKey, out value);
			return (T) value;
		}
		#endregion
	}
	#endregion

	#region Loading Data Class
	public class LoadingData {
		private LoadingDataType key;
		private object value;

		#region Getters & Setters
		public LoadingDataType Key {
			get { return key; }
		}

		public object Value {
			get { return value; }
		}
		#endregion

		#region Constructor
		public LoadingData(LoadingDataType aKey, object aValue) {
			key = aKey;
			value = aValue;
		}
		#endregion
	}
	#endregion

	public enum LoadingDataType {
		Menu,
        Cinematic
	}

}
