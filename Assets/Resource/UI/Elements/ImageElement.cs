using Resource.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Resource.UI {

    [RequireComponent(typeof(Image))]
    public class ImageElement : MonoBehaviour {
        [SerializeField]
        private TransitionType transition = TransitionType.Sprites;
        [SerializeField]
        private TransitionState state = TransitionState.Default;

        public enum TransitionState {
            Default,
            Normal,
            Highlighted,
            Pressed,
            Disabled
        }

        public enum TransitionType {
            Sprites,
            Colors,
            Animation
        }

        // Sprites
        [SerializeField, Visibility("transition", true, TransitionType.Sprites)]
        private Sprite normalSprite;
        [SerializeField, Visibility("transition", true, TransitionType.Sprites)]
        private Sprite highlightsSprite;
        [SerializeField, Visibility("transition", true, TransitionType.Sprites)]
        private Sprite pressedSprite;
        [SerializeField, Visibility("transition", true, TransitionType.Sprites)]
        private Sprite disabledSprite;

        // Colors
        [SerializeField, Visibility("transition", true, TransitionType.Colors)]
        private Color normalColor = new Color(1.0f, 1.0f, 1.0f);
        [SerializeField, Visibility("transition", true, TransitionType.Colors)]
        private Color highlightColor = new Color(0.8f, 0.8f, 1.0f);
        [SerializeField, Visibility("transition", true, TransitionType.Colors)]
        private Color pressedColor = new Color(0.5f, 0.5f, 1.0f);
        [SerializeField, Visibility("transition", true, TransitionType.Colors)]
        private Color disabledColor = new Color(0.2f, 0.2f, 1.0f);

        // Triggers
        [SerializeField, Visibility("transition", true, TransitionType.Animation)]
        private string normalTrigger = "Normal";
        [SerializeField, Visibility("transition", true, TransitionType.Animation)]
        private string highlightedTrigger = "Highlighted";
        [SerializeField, Visibility("transition", true, TransitionType.Animation)]
        private string pressedTrigger = "Pressed";
        [SerializeField, Visibility("transition", true, TransitionType.Animation)]
        private string disabledTrigger = "Disabled";

        private Image image;
        private Animator animator;
        private bool initialized = false;

        #region Getters & Setters
        public Image Image {
            get { return image; }
        }

        public bool Active {
            get { return image.enabled; }
            set { image.enabled = value; }
        }

        public TransitionState State {
            get { return state; }
        }

        public Sprite NormalSprite {
            get { return normalSprite; }
        }

        public Sprite DisabledSprite {
            get { return disabledSprite; }
        }

        public Color NormalColor {
            get { return normalColor; }
        }

        public Color DisabledColor {
            get { return disabledColor; }
        }
        #endregion

        #region Initialization
        private void Awake() {
            Initialize();
        }

        public void Initialize() {
            if (initialized == false) {
                image = GetComponent<Image>();

                if (transition == TransitionType.Animation) {
                    animator = GetComponent<Animator>();
                }
            }

            // Set initial state
            Display(state);

            initialized = true;
        }
        #endregion

        #region Display
        public void Display(TransitionState aState) {
            switch (aState) {
                case TransitionState.Normal:
                    DisplayNormal();
                    break;
                case TransitionState.Highlighted:
                    DisplayHighlighted();
                    break;
                case TransitionState.Pressed:
                    DisplayPressed();
                    break;
                case TransitionState.Disabled:
                    DisplayDisabled();
                    break;
            }

            state = aState;
        }

        private void DisplayNormal() {
            switch (transition) {
                case TransitionType.Sprites:
                    image.sprite = normalSprite;
                    break;
                case TransitionType.Colors:
                    image.color = normalColor;
                    break;
                case TransitionType.Animation:
                    animator.SetTrigger(normalTrigger);
                    break;
            }
        }

        private void DisplayHighlighted() {
            switch (transition) {
                case TransitionType.Sprites:
                    image.sprite = highlightsSprite;
                    break;
                case TransitionType.Colors:
                    image.color = highlightColor;
                    break;
                case TransitionType.Animation:
                    animator.SetTrigger(highlightedTrigger);
                    break;
            }
        }

        private void DisplayPressed() {
            switch (transition) {
                case TransitionType.Sprites:
                    image.sprite = pressedSprite;
                    break;
                case TransitionType.Colors:
                    image.color = pressedColor;
                    break;
                case TransitionType.Animation:
                    animator.SetTrigger(pressedTrigger);
                    break;
            }
        }

        private void DisplayDisabled() {
            switch (transition) {
                case TransitionType.Sprites:
                    image.sprite = disabledSprite;
                    break;
                case TransitionType.Colors:
                    image.color = disabledColor;
                    break;
                case TransitionType.Animation:
                    animator.SetTrigger(disabledTrigger);
                    break;
            }
        }
        #endregion
    }

}
