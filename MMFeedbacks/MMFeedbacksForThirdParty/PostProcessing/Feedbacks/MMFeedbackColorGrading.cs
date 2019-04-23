using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Color Grading")]
    [FeedbackHelp("This feedback allows you to control color grading post exposure, hue shift, saturation and contrast over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Color Grading active, and a MMColorGradingShaker component.")]
    public class MMFeedbackColorGrading : MMFeedback
    {
        [Header("Color Grading")]
        public int Channel = 0;

        public float ShakeDuration = 1f;
        public bool RelativeIntensity = true;

        public AnimationCurve PostExposure = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float PostExposureAmplitude = 0.2f;

        public AnimationCurve HueShift = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float HueShiftAmplitude = -50f;

        public AnimationCurve Saturation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float SaturationAmplitude = 200f;

        public AnimationCurve Contrast = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ContrastAmplitude = 100f;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMColorGradingShakeEvent.Trigger(ShakeDuration, PostExposure, PostExposureAmplitude, HueShift, HueShiftAmplitude, Saturation, SaturationAmplitude,
                    Contrast, ContrastAmplitude, RelativeIntensity, attenuation, Channel);
            }
        }
    }
}
