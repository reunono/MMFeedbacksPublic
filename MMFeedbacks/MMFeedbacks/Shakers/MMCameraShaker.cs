using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

namespace MoreMountains.Feedbacks
{
	[Serializable]
	/// <summary>
	/// Camera shake properties
	/// </summary>
	public struct MMCameraShakeProperties
	{
		public float Duration;
		public float Amplitude;
		public float Frequency;

        public MMCameraShakeProperties(float duration, float amplitude, float frequency)
        {
            Duration = duration;
            Amplitude = amplitude;
            Frequency = frequency;
        }
    }

    public enum MMCameraZoomModes { For, Set, Reset }

    public struct MMCameraZoomEvent
    {
        public delegate void Delegate(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration);
        
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration)
        {
            OnEvent?.Invoke(mode, newFieldOfView, transitionDuration, duration);
        }
    }
    
    public struct MMCameraShakeEvent
    {
        public delegate void Delegate(float duration, float amplitude, float frequency);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration, float amplitude, float frequency)
        {
            OnEvent?.Invoke(duration, amplitude, frequency);
        }
    }
    
	[RequireComponent(typeof(MMWiggle))]
	/// <summary>
	/// A class to add to your camera. It'll listen to MMCameraShakeEvents and will shake your camera accordingly
	/// </summary>
	public class MMCameraShaker : MonoBehaviour
	{
		protected MMWiggle _wiggle;

		/// <summary>
		/// On Awake, grabs the MMShaker component
		/// </summary>
		protected virtual void Awake()
		{
			_wiggle = GetComponent<MMWiggle>();
		}

		/// <summary>
		/// Shakes the camera for Duration seconds, by the desired amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		public virtual void ShakeCamera(float duration, float amplitude, float frequency)
		{
            _wiggle.PositionWiggleProperties.AmplitudeMin = Vector3.one * -amplitude;
            _wiggle.PositionWiggleProperties.AmplitudeMax = Vector3.one * amplitude;
            _wiggle.PositionWiggleProperties.FrequencyMin = frequency;
            _wiggle.PositionWiggleProperties.FrequencyMax = frequency;
            _wiggle.PositionWiggleProperties.NoiseFrequencyMin = frequency * Vector3.one;
            _wiggle.PositionWiggleProperties.NoiseFrequencyMax = frequency * Vector3.one;
            _wiggle.WigglePosition(duration);
		}

		/// <summary>
		/// When a MMCameraShakeEvent is caught, shakes the camera
		/// </summary>
		/// <param name="shakeEvent">Shake event.</param>
		public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency)
		{
			this.ShakeCamera (duration, amplitude, frequency);
		}

		/// <summary>
		/// On enable, starts listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
            MMCameraShakeEvent.Register(OnCameraShakeEvent);
        }

		/// <summary>
		/// On disable, stops listening to events
		/// </summary>
		protected virtual void OnDisable()
		{
            MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
        }

	}
}