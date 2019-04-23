using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Vignette")]
    [FeedbackHelp("This feedback allows you to control vignette intensity over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Vignette active, and a MMVignetteShaker component.")]
    public class MMFeedbackVignette : MMFeedback
    {
        [Header("Vignette")]
        public int Channel = 0;
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float Duration = 0.2f;
        public float Amplitude = 1.0f;
        public bool RelativeIntensity = false;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMVignetteShakeEvent.Trigger(Intensity, Duration, Amplitude, RelativeIntensity, attenuation, Channel);
            }
        }
    }
}
