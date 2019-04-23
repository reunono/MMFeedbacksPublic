using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to control bloom intensity and threshold over time. It requires you have in your scene an object with a PostProcessVolume " +
            "with Bloom active, and a MMBloomShaker component.")]
    [FeedbackPath("PostProcess/Bloom")]
    public class MMFeedbackBloom : MMFeedback
    {
        [Header("Bloom")]
        public int Channel = 0;
        public float ShakeDuration = 0.2f;
        public bool RelativeIntensity = true;
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeIntensityAmplitude = 5.0f;
        public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeThresholdAmplitude = -0.2f;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMBloomShakeEvent.Trigger(ShakeDuration, ShakeIntensity, ShakeIntensityAmplitude, ShakeThreshold, ShakeThresholdAmplitude, RelativeIntensity, attenuation, Channel);
            }
        }
    }
}
