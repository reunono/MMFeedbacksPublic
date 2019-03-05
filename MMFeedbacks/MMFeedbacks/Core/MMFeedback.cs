using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A base class, meant to be extended, defining a Feedback. A Feedback is an action triggered by a MMFeedbacks, usually in reaction to the player's input or actions,
    /// to help communicate both emotion and legibility, improving game feel.
    /// To create a new feedback, extend this class and override its Custom methods, declared at the end of this class. You can look at the many examples for reference.
    /// </summary>
    [AddComponentMenu("")]
    [System.Serializable]
    public abstract class MMFeedback : MonoBehaviour
    {
        /// whether or not this feedback is active
        public bool Active = true;
        /// the name of this feedback to display in the inspector
        public string Label = "MMFeedback";
        /// a number of timing-related values (delay, repeat, etc)
        public MMFeedbackTiming Timing;
        /// the Owner of the feedback, as defined when calling the Initialization method
        public GameObject Owner { get; set; }
        [HideInInspector]
        /// whether or not this feedback is in debug mode
        public bool DebugActive = false;

        protected WaitForSeconds _initialDelayWaitForSeconds;
        protected WaitForSeconds _betweenDelayWaitForSeconds;
        protected float _lastPlayTimestamp = 0f;
        protected int _playsLeft;
        protected bool _initialized = false;
        protected Coroutine _playCoroutine;
        protected Coroutine _infinitePlayCoroutine;
        protected Coroutine _repeatedPlayCoroutine;

        /// <summary>
        /// Initializes the feedback and its timing related variables
        /// </summary>
        /// <param name="owner"></param>
        public virtual void Initialization(GameObject owner)
        {
            _initialized = true;
            Owner = owner;
            _playsLeft = Timing.NumberOfRepeats;
            if (Timing.InitialDelay > 0f)
            {
                _initialDelayWaitForSeconds = new WaitForSeconds(Timing.InitialDelay);
            }
            if (Timing.DelayBetweenRepeats > 0f)
            {
                _betweenDelayWaitForSeconds = new WaitForSeconds(Timing.DelayBetweenRepeats);
            }
            CustomInitialization(owner);
        }

        /// <summary>
        /// Plays the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void Play(Vector3 position, float attenuation = 1.0f)
        {

            if (!_initialized)
            {
                Debug.LogWarning("The " + this + " feedback is being played without having been initialized. Call Initialization() first.");
            }
            
            // we check the cooldown
            if ((Timing.CooldownDuration > 0f) && (Time.time - _lastPlayTimestamp < Timing.CooldownDuration))
            {
                return;
            }

            if (Timing.InitialDelay > 0f) 
            {
                _playCoroutine = StartCoroutine(PlayCoroutine(position, attenuation));
            }
            else
            {
                _lastPlayTimestamp = Time.time;
                RegularPlay(position, attenuation);
            }  
        }
        
        /// <summary>
        /// An internal coroutine delaying the initial play of the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator PlayCoroutine(Vector3 position, float attenuation = 1.0f)
        {
            yield return _initialDelayWaitForSeconds;
            _lastPlayTimestamp = Time.time;
            RegularPlay(position, attenuation);
        }

        /// <summary>
        /// Triggers delaying coroutines if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected virtual void RegularPlay(Vector3 position, float attenuation = 1.0f)
        {
            if (Timing.RepeatForever)
            {
                _infinitePlayCoroutine = StartCoroutine(InfinitePlay(position, attenuation));
                return;
            }
            if (Timing.NumberOfRepeats > 0)
            {
                _repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(position, attenuation));
                return;
            }            
            CustomPlayFeedback(position, attenuation);
        }

        /// <summary>
        /// Internal coroutine used for repeated play without end
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator InfinitePlay(Vector3 position, float attenuation = 1.0f)
        {
            while (true)
            {
                _lastPlayTimestamp = Time.time;
                CustomPlayFeedback(position, attenuation);
                yield return _betweenDelayWaitForSeconds;
            }
        }

        /// <summary>
        /// Internal coroutine used for repeated play
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator RepeatedPlay(Vector3 position, float attenuation = 1.0f)
        {
            while (_playsLeft > 0)
            {
                _lastPlayTimestamp = Time.time;
                _playsLeft--;
                CustomPlayFeedback(position, attenuation);                
                yield return _betweenDelayWaitForSeconds;
            }
            _playsLeft = Timing.NumberOfRepeats;
        }

        /// <summary>
        /// Stops all feedbacks from playing. Will stop repeating feedbacks, and call custom stop implementations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void Stop(Vector3 position, float attenuation = 1.0f)
        {
            if (_playCoroutine != null) { StopCoroutine(_playCoroutine); }
            if (_infinitePlayCoroutine != null) { StopCoroutine(_infinitePlayCoroutine); }
            if (_repeatedPlayCoroutine != null) { StopCoroutine(_repeatedPlayCoroutine); }            

            _lastPlayTimestamp = 0f;
            _playsLeft = Timing.NumberOfRepeats;
            CustomStopFeedback(position, attenuation);
        }

        public virtual void ResetFeedback()
        {
            _playsLeft = Timing.NumberOfRepeats;
            CustomReset();
        }
        
        /// <summary>
        /// This method describes all custom initialization processes the feedback requires, in addition to the main Initialization method
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(GameObject owner) { }

        /// <summary>
        /// This method describes what happens when the feedback gets played
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f);

        /// <summary>
        /// This method describes what happens when the feedback gets stopped
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float attenuation = 1.0f) { }

        /// <summary>
        /// This method describes what happens when the feedback gets reset
        /// </summary>
        protected virtual void CustomReset() { }
    }   
}

