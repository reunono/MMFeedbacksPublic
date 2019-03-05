using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A feedback to bind Unity events to and trigger them when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to bind any type of Unity events to this feebdack's Play, Stop, Initialization and Reset methods.")]
    [FeedbackPath("Events")]
    public class MMFeedbackEvents : MMFeedback
    {
        [Header("Events")]
        /// the events to trigger when the feedback is played
        public UnityEvent PlayEvents;
        /// the events to trigger when the feedback is stopped
        public UnityEvent StopEvents;
        /// the events to trigger when the feedback is initialized
        public UnityEvent InitializationEvents;
        /// the events to trigger when the feedback is reset
        public UnityEvent ResetEvents;

        /// <summary>
        /// On init, triggers the init events
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active && (InitializationEvents != null))
            {
                InitializationEvents.Invoke();
            }
        }

        /// <summary>
        /// On Play, triggers the play events
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (PlayEvents != null))
            {
                PlayEvents.Invoke();                
            }
        }

        /// <summary>
        /// On Stop, triggers the stop events
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (StopEvents != null))
            {
                StopEvents.Invoke();
            }
        }

        /// <summary>
        /// On reset, triggers the reset events
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            if (Active && (ResetEvents != null))
            {
                ResetEvents.Invoke();
            }
        }
    }
}
