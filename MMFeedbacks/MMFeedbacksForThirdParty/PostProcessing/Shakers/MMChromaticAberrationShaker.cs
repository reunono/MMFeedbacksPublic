using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a chromatic aberration post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMChromaticAberrationShaker : MonoBehaviour
    {
        public int Channel = 0;
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeDuration = 0.2f;
        public float ShakeAmplitude = 1.0f;
        public bool RelativeIntensity = false;

        [MMFReadOnly]
        public bool Shaking = false;

        [MMFInspectorButton("StartShaking")]
        public bool TestShakeButton;

        protected ChromaticAberration _chromaticAberration;
        protected PostProcessVolume _volume;
        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;
        protected float _initialIntensity;

        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _chromaticAberration);
            _initialIntensity = _chromaticAberration.intensity;
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
                _chromaticAberration.intensity.Override(_initialIntensity);
            }
        }

        protected virtual void Shake()
        {
            _remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);
           
            _chromaticAberration.intensity.Override(ShakeIntensity.Evaluate(_remappedTimeSinceStart) * ShakeAmplitude);
            if (RelativeIntensity) { _chromaticAberration.intensity.Override(_chromaticAberration.intensity + _initialIntensity); }
           
        }


        public virtual void OnMMChromaticAberrationShakeEvent(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            ShakeDuration = duration;
            ShakeIntensity = intensity;
            ShakeAmplitude = amplitude * attenuation;
            RelativeIntensity = relativeIntensity;
            this.StartShaking();
        }

        protected virtual void OnEnable()
        {
            MMChromaticAberrationShakeEvent.Register(OnMMChromaticAberrationShakeEvent);
        }

        protected virtual void OnDisable()
        {
            MMChromaticAberrationShakeEvent.Unregister(OnMMChromaticAberrationShakeEvent);
        }
    }

    public struct MMChromaticAberrationShakeEvent
    {
        public delegate void Delegate(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            OnEvent?.Invoke(intensity, duration, amplitude, relativeIntensity, attenuation, channel);
        }
    }
}
