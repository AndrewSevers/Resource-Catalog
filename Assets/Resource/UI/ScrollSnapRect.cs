using Resource.Properties;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resource.UI {

    // TODO: Add mask functionality (for reference - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/49806ebb441a04c77d13900b18bc0ce35cd57345/Scripts/Layout/ScrollSnapBase.cs?at=master&fileviewer=file-view-default)
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
        [SerializeField]
        private Axis scrollingAxis;

        [Header("Pages")]
        [SerializeField, Tooltip("Set starting content index - indexed from 0")]
        private int startingIndex = 0;
        [SerializeField]
        private bool pageIndicatorsEnabled = false;
        [SerializeField, Tooltip("Sprite for unselected page"), Visibility("pageIndicatorsEnabled", true)]
        private Sprite inactivePageIcon;
        [SerializeField, Tooltip("Sprite for selected page"), Visibility("pageIndicatorsEnabled", true)]
        private Sprite activePageIcon;
        [SerializeField, Tooltip("Container with page images"), Visibility("pageIndicatorsEnabled", true)]
        private Transform pageIndicatorsTransform;

        [Header("Swiping")]
        [SerializeField, Tooltip("Enable the scrolling to be swipe-able in addition to any button inputs")]
        private bool swipeEnabled = true;
        [SerializeField, Tooltip("Threshold time for fast swipe in seconds"), Visibility("swipeEnabled", true)]
        private float fastSwipeThresholdTime = 0.3f;
        [SerializeField, Tooltip("Threshold time for fast swipe in (unscaled) pixels"), Visibility("swipeEnabled", true)]
        private int fastSwipeThresholdDistance = 100;
        [SerializeField, Tooltip("How fast will page lerp to target position")]
        private float transitionRate = 5.0f;

        [Header("Buttons")]
        [SerializeField]
        private bool buttonsEnabled = false;
        [SerializeField, Tooltip("Button to go to the previous page"), Visibility("buttonsEnabled", true)]
        private Button previousButton;
        [SerializeField, Tooltip("Button to go to the next page"), Visibility("buttonsEnabled", true)]
        private Button nextButton;

        [Header("Events")]
        [SerializeField, Tooltip("Event fires when a user starts to change the selection")]
        private SelectionChangeStartEvent onSelectionChangeStartEvent = new SelectionChangeStartEvent();
        [SerializeField, Tooltip("Event fires as the page changes, while dragging or jumping")]
        private SelectionPageChangedEvent onSelectionPageChangedEvent = new SelectionPageChangedEvent();
        [SerializeField, Tooltip("Event fires when the page settles after a user has dragged")]
        private SelectionChangeEndEvent onSelectionChangeEndEvent = new SelectionChangeEndEvent();

        // fast swipes should be fast and short. If too long, then it is not fast swipe
        private int fastSwipeThresholdMaxLimit;

        private ScrollRect scrollRect;
        private RectTransform content;

        private bool horizontal;

        // number of pages in container
        private int pageCount;
        private int currentPage;

        // whether lerping is in progress and target lerp position
        private bool lerp;
        private Vector2 lerpTarget;

        // target position of every page
        private List<Vector2> pagePositions = new List<Vector2>();

        // in dragging, when dragging started and where it started
        private bool dragging;
        private float timeStamp;
        private Vector2 startPosition;

        // for showing small page icons
        private bool showPageSelection;
        private int previousPageSelectionIndex;

        // container with Image components - one Image for each page
        private List<Image> pageSelectionImages;

        private bool initialized = false;

        public enum Axis {
            Horizontal,
            Vertical
        }

        #region Event Classes
        [System.Serializable]
        public class SelectionChangeStartEvent : UnityEvent { }
        [System.Serializable]
        public class SelectionPageChangedEvent : UnityEvent<int> { }
        [System.Serializable]
        public class SelectionChangeEndEvent : UnityEvent<int> { }
        #endregion

        #region Getters & Setters
        public int CurrentIndex {
            get { return currentPage; }
        }

        public SelectionChangeStartEvent OnSelectionChangeStartEvent {
            get { return onSelectionChangeStartEvent; }
            set { onSelectionChangeStartEvent = value; }
        }

        public SelectionPageChangedEvent OnSelectionPageChangedEvent {
            get { return onSelectionPageChangedEvent; }
            set { onSelectionPageChangedEvent = value; }
        }

        public SelectionChangeEndEvent OnSelectionChangeEndEvent {
            get { return onSelectionChangeEndEvent; }
            set { onSelectionChangeEndEvent = value; }
        }
        #endregion

        #region Initialization
        private void Start() {
            Initialize();
        }

        public void Initialize() {
            if (initialized == false) {
                scrollRect = GetComponent<ScrollRect>();
                content = scrollRect.content;
                pageCount = content.childCount;

                scrollRect.horizontal = false;
                scrollRect.vertical = false;

                switch (scrollingAxis) {
                    case Axis.Horizontal:
                        horizontal = true;
                        scrollRect.horizontal = true;
                        break;
                    case Axis.Vertical:
                        horizontal = false;
                        scrollRect.vertical = true;
                        break;
                }

                if (swipeEnabled == false) {
                    scrollRect.horizontal = false;
                    scrollRect.vertical = false;
                }

                lerp = false;

                // SetupButtons
                SetupButtons();

                // Setup Pages
                SetPagePositions();
                GotoPage(startingIndex);

                // Setup Indicators
                SetupPageIndicators();
                SetPageIndicator(startingIndex);
            }

            initialized = true;
        }
        #endregion

        #region Update
        void Update() {
            // if moving to target position
            if (lerp) {
                // prevent overshooting with values greater than 1
                float decelerate = Mathf.Min(transitionRate * Time.deltaTime, 1f);
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, lerpTarget, decelerate);

                if (Vector2.SqrMagnitude(content.anchoredPosition - lerpTarget) < 0.25f) {
                    // snap to target and stop lerping
                    content.anchoredPosition = lerpTarget;
                    lerp = false;
                    // clear also any scrollrect move that may interfere with our lerping
                    scrollRect.velocity = Vector2.zero;

                    EndPageChange();
                }

                // switches selection icon exactly to correct page
                if (showPageSelection) {
                    SetPageIndicator(GetNearestPage());
                }
            }
        }
        #endregion

        #region Page Management
        private void SetPagePositions() {
            int offset = 0;
            int containerWidth = 0;
            int containerHeight = 0;

            RectTransform _rectTransform = scrollRect.GetComponent<RectTransform>();

            if (horizontal) {
                offset = (int) _rectTransform.rect.width / 2;
                containerWidth = (int) _rectTransform.rect.width * pageCount;
                fastSwipeThresholdMaxLimit = (int) _rectTransform.rect.width;
            } else {
                offset = (int) _rectTransform.rect.height / 2;
                containerHeight = (int) _rectTransform.rect.height * pageCount;
                fastSwipeThresholdMaxLimit = (int) _rectTransform.rect.height;
            }

            // delete any previous settings
            pagePositions.Clear();

            Vector2 _childAnchorPoint = new Vector2(0, 0.5f);

            // iterate through all container childern and set their positions
            for (int i = 0; i < pageCount; i++) {
                RectTransform child = content.GetChild(i).GetComponent<RectTransform>();
                Vector2 childPosition;
                if (horizontal) {
                    childPosition = new Vector2(i * _rectTransform.rect.width - containerWidth / 2 + offset, 0f);
                } else {
                    childPosition = new Vector2(0f, -(i * _rectTransform.rect.height - containerHeight / 2 + offset));
                }

                child.sizeDelta = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
                child.anchoredPosition = childPosition;
                child.anchorMin = child.anchorMax = child.pivot = _childAnchorPoint;

                pagePositions.Add(-childPosition);
            }
        }

        public void GotoPage(int aPageIndex) {
            aPageIndex = Mathf.Clamp(aPageIndex, 0, pageCount - 1);
            content.anchoredPosition = pagePositions[aPageIndex];
            currentPage = aPageIndex;

            lerp = false;

            StartPageChange();
            PageChange();
            EndPageChange();

            AdjustButtonVisibility();
        }

        private void LerpToPage(int aPageIndex) {
            currentPage = aPageIndex;
            lerpTarget = pagePositions[currentPage];
            lerp = true;

            AdjustButtonVisibility();
        }

        private void SetupPageIndicators() {
            if (pageIndicatorsEnabled == false) {
                return;
            }

            // page selection - only if defined sprites for selection icons
            showPageSelection = (inactivePageIcon != null && activePageIcon != null);
            if (showPageSelection) {
                // also container with selection images must be defined and must have exatly the same amount of items as pages container
                if (pageIndicatorsTransform == null || pageIndicatorsTransform.childCount != pageCount) {
                    Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
                    showPageSelection = false;
                } else {
                    previousPageSelectionIndex = -1;
                    pageSelectionImages = new List<Image>();

                    // cache all Image components into list
                    for (int i = 0; i < pageIndicatorsTransform.childCount; i++) {
                        Image image = pageIndicatorsTransform.GetChild(i).GetComponent<Image>();
                        if (image == null) {
                            Debug.LogWarning("Page selection icon at position " + i + " is missing Image component");
                        }
                        pageSelectionImages.Add(image);
                    }
                }
            }
        }

        private void SetPageIndicator(int aPageIndex) {
            if (pageIndicatorsEnabled == false) {
                return;
            }

            // nothing to change
            if (previousPageSelectionIndex == aPageIndex) {
                return;
            }

            // unselect old
            if (previousPageSelectionIndex >= 0) {
                pageSelectionImages[previousPageSelectionIndex].sprite = inactivePageIcon;
                pageSelectionImages[previousPageSelectionIndex].SetNativeSize();
            }

            // select new
            pageSelectionImages[aPageIndex].sprite = activePageIcon;
            pageSelectionImages[aPageIndex].SetNativeSize();

            previousPageSelectionIndex = aPageIndex;
        }

        private int GetNearestPage() {
            // based on distance from current position, find nearest page
            Vector2 currentPosition = content.anchoredPosition;

            float distance = float.MaxValue;
            int nearestPage = currentPage;

            for (int i = 0; i < pagePositions.Count; i++) {
                float testDistance = Vector2.SqrMagnitude(currentPosition - pagePositions[i]);
                if (testDistance < distance) {
                    distance = testDistance;
                    nearestPage = i;
                }
            }

            return nearestPage;
        }

        private void NextPage() {
            if (currentPage < pageCount - 1) {
                if (lerp == false) {
                    StartPageChange();
                }

                LerpToPage(currentPage + 1);
                PageChange();
            }
        }

        private void PreviousPage() {
            if (currentPage > 0) {
                if (lerp == false) {
                    StartPageChange();
                }

                LerpToPage(currentPage - 1);
                PageChange();
            }
        }
        #endregion

        #region Drag Management
        public void OnBeginDrag(PointerEventData aEventData) {
            // if currently lerping, then stop it as user is draging
            lerp = false;
            // not dragging yet
            dragging = false;
        }

        public void OnEndDrag(PointerEventData aEventData) {
            // how much was container's content dragged
            float difference;
            if (horizontal) {
                difference = startPosition.x - content.anchoredPosition.x;
            } else {
                difference = -(startPosition.y - content.anchoredPosition.y);
            }

            // test for fast swipe - swipe that moves only +/-1 item
            if (Time.unscaledTime - timeStamp < fastSwipeThresholdTime && Mathf.Abs(difference) > fastSwipeThresholdDistance && Mathf.Abs(difference) < fastSwipeThresholdMaxLimit) {
                if (difference > 0) {
                    NextPage();
                } else {
                    PreviousPage();
                }
            } else {
                // if not fast time, look to which page we got to
                LerpToPage(GetNearestPage());
            }

            dragging = false;
        }

        public void OnDrag(PointerEventData aEventData) {
            if (!dragging) {
                // dragging started
                dragging = true;
                // save time - unscaled so pausing with Time.scale should not affect it
                timeStamp = Time.unscaledTime;
                // save current position of cointainer
                startPosition = content.anchoredPosition;
            } else {
                PageChange();

                if (showPageSelection) {
                    SetPageIndicator(GetNearestPage());
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event fires when the user starts to change the page, either via swipe or button.
        /// </summary>
        internal void StartPageChange() {
            OnSelectionChangeStartEvent.Invoke();
        }

        /// <summary>
        /// Event fires when the currently viewed page changes, also updates while the scroll is moving
        /// </summary>
        internal void PageChange() {
            OnSelectionPageChangedEvent.Invoke(currentPage);
        }

        /// <summary>
        /// Event fires when control settles on a page, outputs the new page number
        /// </summary>
        internal void EndPageChange() {
            OnSelectionChangeEndEvent.Invoke(currentPage);
        }
        #endregion

        #region Buttons
        private void SetupButtons() {
            if (buttonsEnabled == false) {
                return;
            }

            if (nextButton != null) {
                nextButton.onClick.AddListener(NextPage);
            } else {
                Debug.LogError("[ScrollSnapRect] Buttons enabled but \"Next Button\" is Null");
            }

            if (previousButton != null) {
                previousButton.onClick.AddListener(PreviousPage);
            } else {
                Debug.LogError("[ScrollSnapRect] Buttons enabled but \"Previous Button\" is Null");
            }
        }

        public void AdjustButtonVisibility() {
            if (buttonsEnabled) {
                if (nextButton != null) {
                    nextButton.interactable = (currentPage < pageCount - 1);
                }

                if (previousButton != null) {
                    previousButton.interactable = (currentPage > 0);
                }
            } else {
                if (nextButton != null) {
                    nextButton.gameObject.SetActive(false);
                }

                if (previousButton != null) {
                    previousButton.gameObject.SetActive(false);
                }
            }
        }
        #endregion

    }

}
