using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will animate the scale of the target object over time when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("GameObject/Scale")]
    [FeedbackHelp("This feedback will animate the target's scale on the 3 specified animation curves, for the specified duration (in seconds). You can apply a multiplier, that will multiply each animation curve value.")]
    public class MMFeedbackScale : MMFeedback
    {
        [Header("Scale")]
        /// the object to animate
        public Transform AnimateScaleTarget;
        /// the duration of the animation
        public float AnimateScaleDuration = 0.2f;
        /// how much each curve should be multiplied
        public float Multiplier = 1f;
        /// the x scale animation definition
        public AnimationCurve AnimateScaleX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// the y scale animation definition
        public AnimationCurve AnimateScaleY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// the z scale animation definition
        public AnimationCurve AnimateScaleZ = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));

        /// <summary>
        /// On Play, triggers the scale animation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimateScaleTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    StartCoroutine(AnimateScale(AnimateScaleTarget, Vector3.zero, AnimateScaleDuration, AnimateScaleX, AnimateScaleY, AnimateScaleZ, Multiplier));
                }
            }
        }

        protected virtual IEnumerator AnimateScale(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float multiplier = 1f)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            if ((curveX == null) || (curveY == null) || (curveZ == null))
            {
                yield break;
            }

            if (duration == 0f)
            {
                yield break;
            }

            float journey = 0f;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                vector.x = curveX.Evaluate(percent);
                vector.y = curveY.Evaluate(percent);
                vector.z = curveZ.Evaluate(percent);
                targetTransform.localScale = multiplier * vector;

                journey += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}
