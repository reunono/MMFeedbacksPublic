using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Lens Distortion")]
    [FeedbackHelp("This feedback allows you to control lens distortion intensity over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Lens Distortion active, and a MMLensDistortionShaker component.")]
    public class MMFeedbackLensDistortion : MMFeedback
    {
        [Header("Lens Distortion")]
        public int Channel = 0;
        public bool RelativeIntensity = false;
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0),
                                                                    new Keyframe(0.2f, 1),
                                                                    new Keyframe(0.25f, -1),
                                                                    new Keyframe(0.35f, 0.7f),
                                                                    new Keyframe(0.4f, -0.7f),
                                                                    new Keyframe(0.6f, 0.3f),
                                                                    new Keyframe(0.65f, -0.3f),
                                                                    new Keyframe(0.8f, 0.1f),
                                                                    new Keyframe(0.85f, -0.1f),
                                                                    new Keyframe(1, 0));
        public float Duration = 0.8f;
        public float Amplitude = 50f;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMLensDistortionShakeEvent.Trigger(Intensity, Duration, Amplitude, RelativeIntensity, attenuation, Channel);
            }
        }
    }
}
