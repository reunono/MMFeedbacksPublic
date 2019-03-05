using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback changes the timescale by sending a TimeScale event on play
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback triggers a MMTimeScaleEvent, which, if you have a MMTimeManager object in your scene, will be caught and used to modify the timescale according to the specified settings. These settings are the new timescale (0.5 will be twice slower than normal, 2 twice faster, etc), the duration of the timescale modification, and the optional speed at which to transition between normal and altered time scale.")]
    [FeedbackPath("Time/Timescale Modifier")]
    public class MMFeedbackTimescaleModifier : MMFeedback
    {
        [Header("Timescale Modifier")]
        /// the new timescale to apply
        public float TimeScale = 0.5f;
        /// the duration of the timescale modification
        public float TimeScaleDuration = 1f;
        /// whether or not we should lerp the timescale
        public bool TimeScaleLerp = false;
        /// the speed at which to lerp the timescale
        public float TimeScaleLerpSpeed = 1f;

        /// <summary>
        /// On Play, triggers a time scale event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, TimeScaleDuration, TimeScaleLerp, TimeScaleLerpSpeed, false);
            }
        }
    }
}
