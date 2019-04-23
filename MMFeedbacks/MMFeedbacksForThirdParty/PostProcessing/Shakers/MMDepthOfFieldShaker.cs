using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a depth of field post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMDepthOfFieldShaker : MonoBehaviour
    {
        public int Channel = 0;

        public float ShakeDuration = 0.2f;
        public bool RelativeIntensities = false;

        public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeFocusDistanceAmplitude = 1.0f;

        public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeApertureAmplitude = 1.0f;

        public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeFocalLengthAmplitude = 1.0f;
        
        [MMFReadOnly]
        public bool Shaking = false;

        [MMFInspectorButton("StartShaking")]
        public bool TestShakeButton;

        protected DepthOfField _depthOfField;
        protected PostProcessVolume _volume;
        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;

        protected float _initialFocusDistance;
        protected float _initialAperture;
        protected float _initialFocalLength;

        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _depthOfField);
            _initialFocusDistance = _depthOfField.focusDistance;
            _initialAperture = _depthOfField.aperture;
            _initialFocalLength = _depthOfField.focalLength;
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

                _depthOfField.focusDistance.Override(_initialFocusDistance);
                _depthOfField.aperture.Override(_initialAperture);
                _depthOfField.focalLength.Override(_initialFocalLength);
            }
        }

        protected virtual void Shake()
        {
            _remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);

            _depthOfField.focusDistance.Override(ShakeFocusDistance.Evaluate(_remappedTimeSinceStart) * ShakeFocusDistanceAmplitude);
            _depthOfField.aperture.Override(ShakeAperture.Evaluate(_remappedTimeSinceStart) * ShakeApertureAmplitude);
            _depthOfField.focalLength.Override(ShakeFocalLength.Evaluate(_remappedTimeSinceStart) * ShakeFocalLengthAmplitude);
            if (RelativeIntensities)
            {
                _depthOfField.focusDistance.Override(_depthOfField.focusDistance + _initialFocusDistance);
                _depthOfField.aperture.Override(_depthOfField.aperture + _initialAperture);
                _depthOfField.focalLength.Override(_depthOfField.focalLength + _initialFocalLength);
            }
        }


        public virtual void OnDepthOfFieldShakeEvent(float duration, AnimationCurve focusDistanceIntensity, float focusDistanceAmplitude,
                                                        AnimationCurve apertureIntensity, float apertureAmplitude,
                                                        AnimationCurve focalLengthIntensity, float focalLengthAmplitude, float attenuation = 1.0f,
                                                        int channel = 0)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            ShakeDuration = duration;
            ShakeFocusDistance = focusDistanceIntensity;
            ShakeFocusDistanceAmplitude = focusDistanceAmplitude;
            ShakeAperture = apertureIntensity;
            ShakeApertureAmplitude = apertureAmplitude;
            ShakeFocalLength = focalLengthIntensity;
            ShakeFocalLengthAmplitude = focalLengthAmplitude;
            this.StartShaking();
        }

        protected virtual void OnEnable()
        {
            MMDepthOfFieldShakeEvent.Register(OnDepthOfFieldShakeEvent);
        }

        protected virtual void OnDisable()
        {
            MMDepthOfFieldShakeEvent.Unregister(OnDepthOfFieldShakeEvent);
        }
    }

    public struct MMDepthOfFieldShakeEvent
    {
        public delegate void Delegate(float duration, AnimationCurve focusDistanceIntensity, float focusDistanceAmplitude,
                                                        AnimationCurve apertureIntensity, float apertureAmplitude,
                                                        AnimationCurve focalLengthIntensity, float focalLengthAmplitude, float attenuation = 1.0f, int channel = 0);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration, AnimationCurve focusDistanceIntensity, float focusDistanceAmplitude,
                                                        AnimationCurve apertureIntensity, float apertureAmplitude,
                                                        AnimationCurve focalLengthIntensity, float focalLengthAmplitude, float attenuation = 1.0f, int channel = 0)
        {
            OnEvent?.Invoke(duration, focusDistanceIntensity, focusDistanceAmplitude, apertureIntensity, apertureAmplitude, focalLengthIntensity, focalLengthAmplitude, attenuation, channel);
        }
    }
}
