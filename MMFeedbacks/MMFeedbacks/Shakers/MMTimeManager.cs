using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace MoreMountains.Feedbacks
{
	/// <summary>
	/// A list of the methods available to change the current score
	/// </summary>
	public enum MMTimeScaleMethods
	{
		Set,
		For,
		Reset,
        Unfreeze
	}

	/// <summary>
	/// The different settings you can play with on a timescale event
	/// </summary>
	public struct TimeScaleProperties
	{
		public float TimeScale;
		public float Duration;
		public bool Lerp;
		public float LerpSpeed;
        public bool Infinite;
	}

    public struct MMTimeScaleEvent
    {
        public delegate void Delegate(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
        {
            OnEvent?.Invoke(timeScaleMethod, timeScale, duration, lerp, lerpSpeed, infinite);
        }
    }
    
    public struct MMFreezeFrameEvent
    {
        public delegate void Delegate(float duration);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration)
        {
            OnEvent?.Invoke(duration);
        }
    }

	/// <summary>
	/// Put this component in your scene and it'll catch MMFreezeFrameEvents and MMTimeScaleEvents, allowing you to control the flow of time.
	/// </summary>
	public class MMTimeManager : MonoBehaviour
	{		
		[MMFInformationAttribute("Put this component in your scene and it'll catch MMFreezeFrameEvents and MMTimeScaleEvents, allowing you to control the flow of time.", MMFInformationAttribute.InformationType.Info, false)]
		/// The reference timescale, to which the system will go back to after all time is changed
		public float NormalTimescale = 1f;
		[MMFReadOnly]
		/// the current, real time, time scale
		public float CurrentTimeScale = 1f;
		[MMFReadOnly]
		/// the time scale the system is lerping towards
		public float TargetTimeScale = 1f;
		[MMFReadOnly]
		/// whether or not the timescale should be lerping
		public bool LerpTimescale = true;
		[MMFReadOnly]
		/// the speed at which the timescale should lerp towards its target
		public float LerpSpeed;

		[MMFInspectorButtonAttribute("TestButtonToSlowDownTime")]
		/// a test button for the inspector
		public bool TestButton;

		protected Stack<TimeScaleProperties> _timeScaleProperties;
		protected float _frozenTimeLeft = -1f;
		protected TimeScaleProperties _currentProperty;

		/// <summary>
		/// A method used from the inspector to test the system
		/// </summary>
		protected virtual void TestButtonToSlowDownTime()
		{
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0.5f, 3f, true, 1f, false);
		}

		/// <summary>
		/// On start we initialize our stack and store our initial time scale
		/// </summary>
		protected virtual void Start()
		{
			TargetTimeScale = NormalTimescale;
			_timeScaleProperties = new Stack<TimeScaleProperties>();
		}

		/// <summary>
		/// On Update, applies the timescale and resets it if needed
		/// </summary>
		protected virtual void Update()
		{          

            // if we have things in our stack, we handle them, otherwise we reset to the normal timescale
            if (_timeScaleProperties.Count > 0)
			{
				_currentProperty = _timeScaleProperties.Peek();

				TargetTimeScale = _currentProperty.TimeScale;
				LerpSpeed = _currentProperty.LerpSpeed;
				LerpTimescale = _currentProperty.Lerp;
				_currentProperty.Duration -= Time.unscaledDeltaTime;

				_timeScaleProperties.Pop();
				_timeScaleProperties.Push(_currentProperty);

				if (_currentProperty.Duration <= 0f && !_currentProperty.Infinite)
				{
					Unfreeze();
				}
			}
			else
			{
				TargetTimeScale = NormalTimescale;
			}

			// we apply our timescale
			if (LerpTimescale)
			{
				Time.timeScale = Mathf.Lerp(Time.timeScale, TargetTimeScale, Time.unscaledDeltaTime * LerpSpeed);
			}
			else
			{
				Time.timeScale = TargetTimeScale;
			}

			CurrentTimeScale = Time.timeScale;
		}

		/// <summary>
		/// Resets all stacked timescale changes and simply sets the timescale, until further changes
		/// </summary>
		/// <param name="newTimeScale">New time scale.</param>
		protected virtual void SetTimeScale(float newTimeScale)
		{
			_timeScaleProperties.Clear();
			Time.timeScale = newTimeScale;
		}

		/// <summary>
		/// Sets the time scale for the specified properties (duration, time scale, lerp or not, and lerp speed)
		/// </summary>
		/// <param name="timeScaleProperties">Time scale properties.</param>
		protected virtual void SetTimeScale(TimeScaleProperties timeScaleProperties)
        {
            _timeScaleProperties.Push(timeScaleProperties);
		}

		/// <summary>
		/// Resets the time scale to the stored normal timescale
		/// </summary>
		protected virtual void ResetTimeScale()
		{
			Time.timeScale = NormalTimescale;
		}

		/// <summary>
		/// Resets the time scale to the last saved time scale.
		/// </summary>
		protected virtual void Unfreeze()
        {
            if (_timeScaleProperties.Count > 0)
			{
                _timeScaleProperties.Pop();
			}
            else
            {
                ResetTimeScale();
            }
		}

		/// <summary>
		/// Sets the normal time scale.
		/// </summary>
		/// <param name="newNormalTimeScale">New normal time scale.</param>
		public virtual void SetNormalTimeScale(float newNormalTimeScale)
		{
            NormalTimescale = newNormalTimeScale;
		}

		/// <summary>
		/// Catches TimeScaleEvents and acts on them
		/// </summary>
		/// <param name="timeScaleEvent">MMTimeScaleEvent event.</param>
		public virtual void OnTimeScaleEvent(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
        {
            TimeScaleProperties timeScaleProperty = new TimeScaleProperties();
            timeScaleProperty.TimeScale = timeScale;
            timeScaleProperty.Duration = duration;
            timeScaleProperty.Lerp = lerp;
            timeScaleProperty.LerpSpeed = lerpSpeed;
            timeScaleProperty.Infinite = infinite;

            switch (timeScaleMethod)
            {
				case MMTimeScaleMethods.Reset:
					ResetTimeScale ();
					break;

				case MMTimeScaleMethods.Set:
					SetTimeScale (timeScale);
					break;

				case MMTimeScaleMethods.For:
					SetTimeScale (timeScaleProperty);
					break;

                case MMTimeScaleMethods.Unfreeze:
                    Unfreeze();
                    break;
			}
		}

		/// <summary>
		/// When getting a freeze frame event we stop the time
		/// </summary>
		/// <param name="freezeFrameEvent">Freeze frame event.</param>
		public void OnMMFreezeFrameEvent(float duration)
		{
			_frozenTimeLeft = duration;

			TimeScaleProperties properties = new TimeScaleProperties();
			properties.Duration = duration;
			properties.Lerp = false;
			properties.LerpSpeed = 0f;
			properties.TimeScale = 0f;

			SetTimeScale(properties);
		} 

		/// <summary>
		/// On enable, starts listening for FreezeFrame events
		/// </summary>
		void OnEnable()
		{
			MMFreezeFrameEvent.Register(OnMMFreezeFrameEvent);
            MMTimeScaleEvent.Register(OnTimeScaleEvent);
		}

		/// <summary>
		/// On disable, stops listening for FreezeFrame events
		/// </summary>
		void OnDisable()
        {
            MMFreezeFrameEvent.Unregister(OnMMFreezeFrameEvent);
            MMTimeScaleEvent.Unregister(OnTimeScaleEvent);
        }		
	}
}
