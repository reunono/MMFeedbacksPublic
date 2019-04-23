using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a bloom post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMBloomShaker : MonoBehaviour
    {
        public int Channel = 0;
        public float ShakeDuration = 0.2f;
        public bool RelativeIntensity = false;
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeIntensityAmplitude = 1.0f;
        public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeThresholdAmplitude = 1.0f;

        [MMFReadOnly]
        public bool Shaking = false;

        [MMFInspectorButton("StartShaking")]
        public bool TestShakeButton;

        protected Bloom _bloom;
        protected PostProcessVolume _volume;

        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;

        protected float _initialIntensity;
        protected float _initialThreshold;

        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _bloom);
            _initialIntensity = _bloom.intensity;
            _initialThreshold = _bloom.threshold;
            Shaking = false;
        }

        public virtual void StartShaking()
        {
            if (Shaking)
            {
                return;
            }
            else
            {
                _shakeStartedTimestamp = Time.time;
                Shaking = true;
            }
        }

        protected virtual void Update()
        {
            if (Shaking)
            {
                Shake();
            }

            if (Shaking && (Time.time - _shakeStartedTimestamp > ShakeDuration))
            {
                Shaking = false;

                _bloom.intensity.Override(_initialIntensity);
                _bloom.threshold.Override(_initialThreshold);
            }
        }

        protected virtual void Shake()
        {
            _remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);

            _bloom.intensity.Override(ShakeIntensity.Evaluate(_remappedTimeSinceStart) * ShakeIntensityAmplitude);
            _bloom.threshold.Override(ShakeThreshold.Evaluate(_remappedTimeSinceStart) * ShakeThresholdAmplitude);
            if (RelativeIntensity) { _bloom.intensity.Override(_bloom.intensity + _initialIntensity); }
            if (RelativeIntensity) { _bloom.threshold.Override(_bloom.threshold + _initialThreshold); }
        }


        public virtual void OnBloomShakeEvent(float duration, AnimationCurve intensity, float intensityAmplitude, AnimationCurve threshold, float thresholdAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            ShakeDuration = duration;
            ShakeIntensity = intensity;
            ShakeIntensityAmplitude = intensityAmplitude * attenuation;
            ShakeThreshold = threshold;
            ShakeThresholdAmplitude = thresholdAmplitude * attenuation;
            RelativeIntensity = relativeIntensity;
            this.StartShaking();
        }

        protected virtual void OnEnable()
        {
            MMBloomShakeEvent.Register(OnBloomShakeEvent);
        }

        protected virtual void OnDisable()
        {
            MMBloomShakeEvent.Unregister(OnBloomShakeEvent);
        }
    }

    public struct MMBloomShakeEvent
    {
        public delegate void Delegate(float duration, AnimationCurve intensity, float intensityAmplitude, AnimationCurve threshold, float thresholdAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration, AnimationCurve intensity, float intensityAmplitude, AnimationCurve threshold, float thresholdAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            OnEvent?.Invoke(duration, intensity, intensityAmplitude, threshold, thresholdAmplitude, relativeIntensity, attenuation, channel);
        }
    }
}
