using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// this feedback will let you animate the position of 
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will animate the target object's position over time, for the specified duration, from the chosen initial position to the chosen destination. These can either be relative Vector3 offsets from the Feedback's position, or Transforms. If you specify transforms, the Vector3 values will be ignored.")]
    [FeedbackPath("GameObject/Position")]
    public class MMFeedbackPosition : MMFeedback
    {
        [Header("Position")]
        /// the object this feedback will animate the position for
        public GameObject AnimatePositionTarget;
        /// the duration of the animation on play
        public float AnimatePositionDuration = 0.2f;
        /// the acceleration of the movement
        public AnimationCurve AnimatePositionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.1f, 0.05f), new Keyframe(0.9f, 0.95f), new Keyframe(1, 1));
        /// the initial position
        public Vector3 InitialPosition;
        /// the destination position
        public Vector3 DestinationPosition;
        /// the initial transform - if set, takes precedence over the Vector3 above
        public Transform InitialPositionTransform;
        /// the destination transform - if set, takes precedence over the Vector3 above
        public Transform DestinationPositionTransform;

        /// <summary>
        /// On init, we set our initial and destination positions (transform will take precedence over vector3s)
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active)
            {
                if (AnimatePositionTarget == null)
                {
                    Debug.LogWarning("The animate position target for " + this + " is null, you have to define it in the inspector");
                    return;
                }

                if (InitialPositionTransform != null) 
                {
                    InitialPosition = InitialPositionTransform.position;
                }
                else
                {
                    InitialPosition = AnimatePositionTarget.transform.position + InitialPosition;
                }

                if (DestinationPositionTransform != null)
                {
                    DestinationPosition = DestinationPositionTransform.position;
                }
                else
                {
                    DestinationPosition = AnimatePositionTarget.transform.position + DestinationPosition;
                }
            }
        }

        /// <summary>
        /// On Play, we move our object from A to B
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimatePositionTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    StartCoroutine(MoveFromTo(AnimatePositionTarget, InitialPosition, DestinationPosition, AnimatePositionDuration, AnimatePositionCurve));
                }
            }
        }

        /// <summary>
		/// Moves an object from point A to point B in a given time
		/// </summary>
		/// <param name="movingObject">Moving object.</param>
		/// <param name="pointA">Point a.</param>
		/// <param name="pointB">Point b.</param>
		/// <param name="duration">Time.</param>
		protected virtual IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float duration, AnimationCurve curve = null)
        {
            float journey = 0f;
            Vector3 newPosition;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                newPosition = Vector3.Lerp(pointA, pointB, curve.Evaluate(percent));

                movingObject.transform.position = newPosition;

                journey += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
