using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a color grading post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMColorGradingShaker : MonoBehaviour
    {
        public int Channel = 0;

        public float ShakeDuration = 1f;
        public bool RelativeIntensity = true;

        public AnimationCurve ShakePostExposure = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakePostExposureAmplitude = 0.2f;

        public AnimationCurve ShakeHueShift = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeHueShiftAmplitude = -50f;

        public AnimationCurve ShakeSaturation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeSaturationAmplitude = 200f;

        public AnimationCurve ShakeContrast = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeContrastAmplitude = 100f;

        [MMFReadOnly]
        public bool Shaking = false;

        [MMFInspectorButton("StartShaking")]
        public bool TestShakeButton;

        protected ColorGrading _colorGrading;
        protected PostProcessVolume _volume;
        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;

        protected float _initialPostExposure;
        protected float _initialHueShift;
        protected float _initialSaturation;
        protected float _initialContrast;

        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _colorGrading);
            _initialPostExposure = _colorGrading.postExposure;
            _initialHueShift = _colorGrading.hueShift;
            _initialSaturation = _colorGrading.saturation;
            _initialContrast = _colorGrading.contrast;
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

                _colorGrading.postExposure.Override(_initialPostExposure);
                _colorGrading.hueShift.Override(_initialHueShift);
                _colorGrading.saturation.Override(_initialSaturation);
                _colorGrading.contrast.Override(_initialContrast);
            }
        }

        protected virtual void Shake()
        {
            _remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);

            _colorGrading.postExposure.Override(ShakePostExposure.Evaluate(_remappedTimeSinceStart) * ShakePostExposureAmplitude);
            _colorGrading.hueShift.Override(ShakeHueShift.Evaluate(_remappedTimeSinceStart) * ShakeHueShiftAmplitude);
            _colorGrading.saturation.Override(ShakeSaturation.Evaluate(_remappedTimeSinceStart) * ShakeSaturationAmplitude);
            _colorGrading.contrast.Override(ShakeContrast.Evaluate(_remappedTimeSinceStart) * ShakeContrastAmplitude);

            if (RelativeIntensity) { _colorGrading.postExposure.Override(_colorGrading.postExposure + _initialPostExposure); }
            if (RelativeIntensity) { _colorGrading.hueShift.Override(_colorGrading.hueShift + _initialHueShift); }
            if (RelativeIntensity) { _colorGrading.saturation.Override(_colorGrading.saturation + _initialSaturation); }
            if (RelativeIntensity) { _colorGrading.contrast.Override(_colorGrading.contrast + _initialContrast); }
        }


        public virtual void OnMMColorGradingShakeEvent(float duration, AnimationCurve postExposure, float postExposureAmplitude, AnimationCurve hueShift, float hueShiftAmplitude,
            AnimationCurve saturation, float saturationAmplitude, AnimationCurve contrast, float contrastAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            ShakeDuration = duration;
            RelativeIntensity = relativeIntensity;

            ShakePostExposure = postExposure;
            ShakePostExposureAmplitude = postExposureAmplitude * attenuation;

            ShakeHueShift = hueShift;
            ShakeHueShiftAmplitude = hueShiftAmplitude * attenuation;

            ShakeSaturation = saturation;
            ShakeSaturationAmplitude = saturationAmplitude * attenuation;

            ShakeContrast = contrast;
            ShakeContrastAmplitude = contrastAmplitude * attenuation;
            
            this.StartShaking();
        }

        protected virtual void OnEnable()
        {
            MMColorGradingShakeEvent.Register(OnMMColorGradingShakeEvent);
        }

        protected virtual void OnDisable()
        {
            MMColorGradingShakeEvent.Unregister(OnMMColorGradingShakeEvent);
        }
    }

    public struct MMColorGradingShakeEvent
    {
        public delegate void Delegate(float duration, AnimationCurve postExposure, float postExposureAmplitude, AnimationCurve hueShift, float hueShiftAmplitude,
            AnimationCurve saturation, float saturationAmplitude, AnimationCurve contrast, float contrastAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration, AnimationCurve postExposure, float postExposureAmplitude, AnimationCurve hueShift, float hueShiftAmplitude,
            AnimationCurve saturation, float saturationAmplitude, AnimationCurve contrast, float contrastAmplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            OnEvent?.Invoke(duration, postExposure, postExposureAmplitude, hueShift, hueShiftAmplitude,
            saturation, saturationAmplitude, contrast, contrastAmplitude, relativeIntensity, attenuation, channel);
        }
    }
}
