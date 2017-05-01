using System.Collections.Generic;
using UnityEngine;

namespace Resource.Touching {

    public class TouchSimulator : MonoBehaviour {
		[SerializeField]
		private Texture cursorNormal;
		[SerializeField]
		private Texture cursorHighlight;
		[SerializeField]
		private Texture cursorHeld;

#if UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
		private Vector2 fingerSize = new Vector2(113.0f / 2, 128.0f / 2);
		private bool dragging;
		
		private static List<Touch> touches = new List<Touch>();
		private static int fingerId;

		#region Getters & Setters
		public static Touch[] Touches {
			get { return touches.ToArray(); }
		}

		public static int TouchCount {
			get { return touches.Count; }
		}
		#endregion

		#region Initialization
		void Start() {
			TouchSimulator[] touchSimulators = FindObjectsOfType<TouchSimulator>();
			if (touchSimulators.Length > 1) {
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);

			if (touches == null) {
				touches = new List<Touch>();
				fingerId = 0;
			}
		}
		#endregion

		#region Update
		void Update() {
			// Create an active finger for taps, drags, and swipes
			if (Input.GetMouseButtonDown(0)) {
				int touchIndex = touches.FindIndex(t => t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved);
				Touch touch = (touchIndex >= 0) ? touches[touchIndex] : new Touch();

				touch.position = Input.mousePosition;

				touch.phase = TouchPhase.Began;
				touch.type = TouchType.Direct;
				touch.fingerId = fingerId++;
				touches.Add(touch);
				dragging = true;

				return;
			// Remove the active finger
			} else if (Input.GetMouseButtonUp(0)) {
				int touchIndex = touches.FindIndex(t => t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved);
				if (touchIndex >= 0) {
					Touch touchToUpdate = touches[touchIndex];

					Touch touch = new Touch();
					touch.position = touchToUpdate.position;
					touch.fingerId = touchToUpdate.fingerId;
					touch.type = touchToUpdate.type;
					touch.phase = TouchPhase.Ended;

					touches[touchIndex] = touch;
				}

				dragging = false;
			}

			// Add stationary/held finger
			if (Input.GetMouseButtonUp(1)) {
				Touch touch = new Touch();
                
				touch.position = Input.mousePosition;

				touch.phase = TouchPhase.Began;
				touch.type = TouchType.Indirect;
				touch.fingerId = fingerId++;

				touches.Add(touch);
			}

			// Move active finger
			if (dragging) {
				int touchIndex = touches.FindIndex(t => t.phase == TouchPhase.Moved);
				if (touchIndex >= 0) {
					Touch touchToUpdate = touches[touchIndex];
					
					Touch touch = new Touch();
					touch.fingerId = touchToUpdate.fingerId;
					touch.type = touchToUpdate.type;
					touch.phase = touchToUpdate.phase;
                    
					touch.position = Input.mousePosition;

					touches[touchIndex] = touch;
				}
			}
		}
		#endregion

		#region Late Update
		void LateUpdate() {
			// Change the state of fingers
			for (int i = touches.Count - 1; i >= 0; i--) {
				Touch touchToUpdate = touches[i];
				switch (touchToUpdate.phase) {
					case TouchPhase.Began:
						Touch touch = new Touch();
						touch.fingerId = touchToUpdate.fingerId;
						touch.type = touchToUpdate.type;
						touch.position = touchToUpdate.position;

						touch.phase = (touch.type == TouchType.Direct) ? TouchPhase.Moved : TouchPhase.Stationary;

						touches[i] = touch;
						break;
					case TouchPhase.Canceled:
						break;
					case TouchPhase.Ended:
						touches.RemoveAt(i);
						break;
					case TouchPhase.Moved:
						break;
					case TouchPhase.Stationary:
						break;
				}
			}
		}
		#endregion

		#region GUI
		void OnGUI() {
			float buttonWidth = Screen.width / 8;
			float buttonHeight = Screen.height / 16;
			if (GUI.Button(new Rect(Screen.width - buttonWidth, 0.0f, buttonWidth, buttonHeight), "Reset Touches")) {
				touches.Clear();
				fingerId = 0;
			}

			foreach (Touch touch in touches) {
				switch (touch.phase) {
					case TouchPhase.Began:
                        Vector2 screenTouchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);
                        GUI.DrawTexture(new Rect(screenTouchPosition, fingerSize), cursorNormal);
						break;
					case TouchPhase.Canceled:
						break;
					case TouchPhase.Ended:
						break;
					case TouchPhase.Moved:
                        screenTouchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);
                        GUI.DrawTexture(new Rect(screenTouchPosition, fingerSize), cursorNormal);
						break;
					case TouchPhase.Stationary:
                        screenTouchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);
                        GUI.DrawTexture(new Rect(screenTouchPosition, fingerSize), cursorHeld);
						break;
				}
			}
		}
		#endregion
#endif
	}

}
