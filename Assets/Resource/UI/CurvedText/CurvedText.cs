using Resource.Properties;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resource.UI {

    [ExecuteInEditMode]
    public class CurvedText : Text {
        [Header("Curve Data")]
        [SerializeField, Min(0.001f)]
        private float radius = 0.5f;
        [SerializeField, Min(0.001f)]
        private float wrapAngle = 360.0f;
        [SerializeField]
        private float scaleFactor = 100.0f;
        [SerializeField]
        private Direction curve = Direction.Down;
        [SerializeField, Tooltip("Flip the text horizontally")]
        private bool flipHorizontally;

        private float _radius = -1;
        private float _scaleFactor = -1;
        private float _circumference = -1;

        private enum Direction {
            Up,
            Down
        }

        #region Getters & Setters
        private float Circumference {
            get {
                if (_radius != radius || _scaleFactor != scaleFactor) {
                    _radius = radius;
                    _scaleFactor = scaleFactor;
                    _circumference = 2.0f * Mathf.PI * radius * scaleFactor;
                }

                return _circumference;
            }
        }
        #endregion

        #region Update
#if UNITY_EDITOR
        private void Update() {
            if (radius <= 0.0f) {
                radius = 0.001f;
            }

            if (scaleFactor <= 0.0f) {
                scaleFactor = 0.001f;
            }

            rectTransform.sizeDelta = new Vector2(Circumference * wrapAngle / 360.0f, rectTransform.sizeDelta.y);
        }
#endif
        #endregion

        #region Utilities
        protected override void OnPopulateMesh(VertexHelper aVertexHelper) {
            base.OnPopulateMesh(aVertexHelper);

            List<UIVertex> stream = new List<UIVertex>();

            aVertexHelper.GetUIVertexStream(stream);

            for (int i = 0; i < stream.Count; i++) {
                UIVertex vertex = stream[i];

                float percentCircumference = -(vertex.position.x / Circumference);
                if (flipHorizontally) {
                    percentCircumference = -percentCircumference;
                }

                switch (curve) {
                    case Direction.Up:
                        Vector3 offset = Quaternion.Euler(0.0f, 0.0f, -percentCircumference * 360.0f) * Vector3.up;

                        vertex.position = offset * -radius * scaleFactor + offset * vertex.position.y;
                        vertex.position += Vector3.down * -radius * scaleFactor;
                        break;
                    case Direction.Down:
                        offset = Quaternion.Euler(0.0f, 0.0f, percentCircumference * 360.0f) * Vector3.up;

                        vertex.position = offset * radius * scaleFactor + offset * vertex.position.y;
                        vertex.position += Vector3.down * radius * scaleFactor;
                        break;
                }

                stream[i] = vertex;
            }

            aVertexHelper.AddUIVertexTriangleStream(stream);
        }
        #endregion

    }

}
