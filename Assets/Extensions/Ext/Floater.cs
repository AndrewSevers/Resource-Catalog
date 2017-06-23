using Extensions.Pooling;
using Extensions.Properties;
using System;
using System.Collections;
using UnityEngine;

namespace Extensions {

    public class Floater : PoolableObject {
        [SerializeField, LimitedVector3(-100, 100, -100, 100, -100, 100), Tooltip("Possible offset to use for starting position")]
        private LimitedVector3 appearanceOffset;
        [SerializeField, LimitedVector3(-100, 100, -100, 100, -100, 100), Tooltip("The area (from origin) that this floater can move to when moving in random direction")]
        protected LimitedVector3 movementArea;
        [SerializeField]
        private float movementDuration = 1.0f;
        [SerializeField]
        private bool ease;
        [SerializeField, Visibility("ease", true)]
        private AnimationCurve easingCurve;

        #region Getters & Setters
        public LimitedVector3 AppearanceOffset {
            get { return appearanceOffset; }
        }

        public LimitedVector3 MovementArea {
            get { return movementArea; }
        }
        #endregion

        #region Spawning
        public void Spawn(Vector3 aPosition) {
            transform.position = aPosition + AppearanceOffset.Random();
        }
        #endregion

        #region Launching
        /// <summary>
        /// Launch the floater to a random position based on its current position and apperance force
        /// </summary>
        /// <param name="aDuration">How long it takes to move to the randomly selected location. If not set it will use the default duration on the object</param>
        /// <param name="aCompletionAction">Action to run on completion of luanch</param>
        public void Launch(float aDuration = 0, Action<Floater> aCompletionAction = null) {
            StartCoroutine(ProcessMoveInDirection(aDuration, aCompletionAction));
        }

        /// <summary>
        /// Launch the floater and move to the provided destination
        /// </summary>
        /// <param name="aDestination">Destination to move to</param>
        /// <param name="aMoveDirectly">If true this object will skip moving based on the appearance force and just move straight to the target</param>
        /// <param name="aSpaceType">What space type to move within (World, Screen, Viewport</param>
        /// <param name="aCompletionAction">Action to run on completion of luanch</param>
        public void Launch(Vector3 aDestination, bool aMoveDirectly = false, SpaceType aSpaceType = SpaceType.World, Action<Floater> aCompletionAction = null) {
            StartCoroutine(ProcessMoveToDestination(aDestination, aMoveDirectly, aSpaceType, aCompletionAction));
        }

        /// <summary>
        /// Launch the floater and move to the provided target
        /// </summary>
        /// <param name="aTarget">Target to move to</param>
        /// <param name="aMoveDirectly">If true this object will skip moving based on the appearance force and just move straight to the target</param>
        /// <param name="aSpaceType">What space type to move within (World, Screen, Viewport</param>
        /// <param name="aCompletionAction">Action to run on completion of luanch</param>
        public void Launch(GameObject aTarget, bool aMoveDirectly = false, SpaceType aSpaceType = SpaceType.World, Action<Floater> aCompletionAction = null) {
            StartCoroutine(ProcessMoveToObject(aTarget, aMoveDirectly, aSpaceType, aCompletionAction));
        }
        #endregion

        #region Movement
        /// <summary>
        /// Move in a direction based on appearance force
        /// </summary>
        private IEnumerator ProcessMoveInDirection(float aDuration, Action<Floater> aCompletionAction) {
            float time = 0;
            Vector3 startingPosition = transform.position;
            Vector3 destination = startingPosition + movementArea.Random();

            aDuration = (aDuration > 0) ? aDuration : movementDuration;

            do {
                time += Time.deltaTime / aDuration;

                transform.position = Lerp(startingPosition, destination, time, SpaceType.World);

                yield return null;
            } while (time < 1);

            if (aCompletionAction != null) {
                aCompletionAction(this);
            }
        }

        /// <summary>
        /// Move to the provided destination
        /// </summary>
        private IEnumerator ProcessMoveToDestination(Vector3 aDestination, bool aMoveDirectly, SpaceType aSpaceType, Action<Floater> aCompletionAction) {
            if (aMoveDirectly == false) {
                yield return StartCoroutine(ProcessMoveInDirection(0.4f, null));
            }

            float time = 0;
            Vector3 startingPosition = transform.position;

            do {
                time += Time.deltaTime / movementDuration;

                transform.position = Lerp(startingPosition, aDestination, time, aSpaceType);

                yield return null;
            } while (time < 1);

            if (aCompletionAction != null) {
                aCompletionAction(this);
            }
        }

        /// <summary>
        /// Move to the provided destination
        /// </summary>
        private IEnumerator ProcessMoveToObject(GameObject aTarget, bool aMoveDirectly, SpaceType aSpaceType, Action<Floater> aCompletionAction) {
            if (aMoveDirectly == false) {
                yield return StartCoroutine(ProcessMoveInDirection(0.4f, null));
            }

            float time = 0;
            Vector3 startingPosition = transform.position;

            do {
                time += Time.deltaTime / movementDuration;

                transform.position = Lerp(startingPosition, aTarget.transform.position, time, aSpaceType);

                yield return null;
            } while (time < 1);

            if (aCompletionAction != null) {
                aCompletionAction(this);
            }
        }
        #endregion

        #region Utilites
        private Vector3 Lerp(Vector3 aOrigin, Vector3 aDestination, float aTime, SpaceType aSpaceType) {
            switch (aSpaceType) {
                case SpaceType.Screen:
                    if (ease) {
                        return Vector3.Lerp(aOrigin, Camera.main.ScreenToWorldPoint(aDestination), easingCurve.Evaluate(aTime));
                    } else {
                        return Vector3.Lerp(aOrigin, Camera.main.ScreenToWorldPoint(aDestination), aTime);
                    }
                case SpaceType.Viewport:
                    if (ease) {
                        return Vector3.Lerp(aOrigin, Camera.main.ViewportToWorldPoint(aDestination), easingCurve.Evaluate(aTime));
                    } else {
                        return Vector3.Lerp(aOrigin, Camera.main.ViewportToWorldPoint(aDestination), aTime);
                    }
                default:
                    if (ease) {
                        return Vector3.Lerp(aOrigin, aDestination, easingCurve.Evaluate(aTime));
                    } else {
                        return Vector3.Lerp(aOrigin, aDestination, aTime);
                    }
            }
        }
        #endregion
    }

    public enum SpaceType {
        World,
        Screen,
        Viewport
    }

}
