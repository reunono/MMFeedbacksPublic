using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to control depth of field focus distance, aperture and focal length over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Depth of Field active, and a MMDepthOfFieldShaker component.")]
    [FeedbackPath("PostProcess/Depth Of Field")]
    public class MMFeedbackDepthOfField : MMFeedback
    {
        [Header("Depth Of Field")]
        public int Channel = 0;

        public float ShakeDuration = 0.2f;
        public bool RelativeIntensities = false;

        public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeFocusDistanceAmplitude = 1.0f;

        public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeApertureAmplitude = 1.0f;

        public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeFocalLengthAmplitude = 1.0f;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMDepthOfFieldShakeEvent.Trigger(ShakeDuration, ShakeFocusDistance, ShakeFocusDistanceAmplitude, ShakeAperture, ShakeApertureAmplitude, ShakeFocalLength, ShakeFocalLengthAmplitude, attenuation, Channel);
            }
        }
    }
}
