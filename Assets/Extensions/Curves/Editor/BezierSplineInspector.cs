using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Extensions.Utils {

    [CustomEditor(typeof(BezierSpline))]
	public class BezierSplineInspector : UnityEditor.Editor {
		private const int stepsPerCurve = 10;
		private const float directionScale = 0.5f;
		private const float handleSize = 0.04f;
		private const float pickSize = 0.06f;

		private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

		private BezierSpline spline;
		private Transform handleTransform;
		private Quaternion handleRotation;
		private Dictionary<KeyCode, bool> inputs = new Dictionary<KeyCode, bool>();
		private int selectedIndex = -1;
		private bool displayPoints;
		private bool showPointDetails = true;
        private bool showOptions = false;

        #region Inspector GUI
        public override void OnInspectorGUI() {
			spline = target as BezierSpline;
			EditorGUI.BeginChangeCheck();
			bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Toggle Loop");
				EditorUtility.SetDirty(spline);
				spline.Loop = loop;
			}
            
			showPointDetails = EditorGUILayout.Toggle("Show Point Details", showPointDetails);            
            showOptions = EditorGUILayout.Toggle("Show Options", showOptions);

            SceneView.RepaintAll();

            DrawPointsInspector();

			if (GUILayout.Button("Add Curve")) {
				Undo.RecordObject(spline, "Add Curve");
				spline.AddCurve();
				EditorUtility.SetDirty(spline);
			}
		}

		private void DrawPointsInspector() {
			displayPoints = EditorGUILayout.Foldout(displayPoints, "Display All Points");
			
			// Draw all points
			if (displayPoints) {
				for (int i = 0; i < spline.ControlPointCount; i++) {
					// Highlight selected point
					if (i == selectedIndex && selectedIndex < spline.Points.Length) {
						DrawSelectedPointInspector();
					// None selected points
					} else {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Point " + i, GUILayout.Width(EditorGUIUtility.labelWidth));
						if (GUILayout.Button("Select")) {
							selectedIndex = i;
							SceneView.RepaintAll();
						}
                        
                        Color originalColor = GUI.color;
                        GUI.color = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(18.0f))) {
                            spline.RemovePoint(i);
                            SceneView.RepaintAll();
                        }
                        GUI.color = originalColor;

                        EditorGUILayout.EndHorizontal();
					}
				}
			// Only draw selected points
			} else {
				if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
					DrawSelectedPointInspector();
				}
			}
		}

		private void DrawSelectedPointInspector() {
			EditorGUILayout.BeginVertical(GUI.skin.button);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Point " + selectedIndex);

            Color originalColor = GUI.color;
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(18))) {
                spline.RemovePoint(selectedIndex);
            }
            GUI.color = originalColor;

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
			Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(selectedIndex, point);
			}

			EditorGUI.BeginChangeCheck();
			BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Change Point Mode");
				spline.SetControlPointMode(selectedIndex, mode);
				EditorUtility.SetDirty(spline);
			}
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Scene GUI
		private void OnSceneGUI() {
			UpdateKeys();

			spline = target as BezierSpline;
			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
			for (int i = 1; i < spline.ControlPointCount; i += 3) {
				Vector3 p1 = ShowPoint(i);
				Vector3 p2 = ShowPoint(i + 1);
				Vector3 p3 = ShowPoint(i + 2);

				Handles.color = Color.gray;
				Handles.DrawLine(p0, p1);
				Handles.DrawLine(p2, p3);

				Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
				p0 = p3;
			}
			ShowDirections();

            if (showOptions) {
                ShowOptions();
            }
        }

		private void ShowDirections() {
			Handles.color = Color.green;
			Vector3 point = spline.GetPoint(0f);
			Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
			int steps = stepsPerCurve * spline.CurveCount;
			for (int i = 1; i <= steps; i++) {
				point = spline.GetPoint(i / (float) steps);
				Handles.DrawLine(point, point + spline.GetDirection(i / (float) steps) * directionScale);
			}
		}

		private Vector3 ShowPoint(int index) {
			Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
			float size = HandleUtility.GetHandleSize(point);
			if (index == 0) {
				size *= 2f;
			}

			// Color the handles based on the control mode for the point
			Handles.color = modeColors[(int) spline.GetControlPointMode(index)];
			if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
				selectedIndex = index;
				Repaint();
			}

            // Add the transform handle/gizmo to the selected point
            if (selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
				
				Vector3 offset = Handles.DoPositionHandle(point, handleRotation) - point;

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);

                    if (IsKeyPressed(KeyCode.A)) {
                        for (int i = 0; i < spline.ControlPointCount; i += 3) {
                            if (index != i) {
                                Vector3 newPoint = handleTransform.TransformPoint(spline.GetControlPoint(i));
                                spline.SetControlPoint(i, handleTransform.InverseTransformPoint(newPoint + offset));
                            }
                        }
                    } else if (IsKeyPressed(KeyCode.C)) {
                        for (int i = 0; i < spline.ControlPointCount; i += 3) {
                            if (index != i && (i == index - 3 || i == index + 3)) {
                                Vector3 newPoint = handleTransform.TransformPoint(spline.GetControlPoint(i));
                                spline.SetControlPoint(i, handleTransform.InverseTransformPoint(newPoint + offset));
                            }
                        }
                    }

					spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point + offset));
				}

				if (showPointDetails) {
					ShowPointDescription(point, index);
				}
			}

			return point;
		}

		private void ShowPointDescription(Vector3 aPoint, int aIndex) {
			Rect guiRect = HandleUtility.WorldPointToSizedRect(aPoint, new GUIContent(), GUI.skin.textField);    // Convert the GameObject's position to screen Size Rect
			guiRect.x += 25;
			guiRect.y -= 50;
			guiRect.width = 175;
			guiRect.height = 40;

			Handles.BeginGUI();
			GUILayout.BeginArea(guiRect);
			GUILayout.Label(string.Format("Point {0} \nPosition: {1}", aIndex, aPoint), GUI.skin.textField);

            GUILayout.EndArea();
			Handles.EndGUI();
		}

        private void ShowOptions() {
            GUILayout.BeginArea(new Rect(10.0f, 10.0f, 225, 80));

            GUILayout.BeginVertical();
            GUILayout.Label("Controls", GUI.skin.textField);
            GUILayout.Label(string.Format("{0}\n{1}", "A+Mouse0: Move all points", "C+Mouse0: Move connected points"), GUI.skin.textField);
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }
		#endregion

		#region Utility Functions
		private void UpdateKeys() {
			switch (Event.current.type) {
				case EventType.KeyDown:
					inputs[Event.current.keyCode] = true;
					break;
				case EventType.KeyUp:
					inputs[Event.current.keyCode] = false;
					break;
			}
		}

		private bool IsKeyPressed(KeyCode aKeyCode) {
			bool isPressed = false;
			inputs.TryGetValue(aKeyCode, out isPressed);
			return isPressed; 
		}
		#endregion

	}

}
