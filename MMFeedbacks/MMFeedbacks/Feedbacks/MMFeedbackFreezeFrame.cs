using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will trigger a freeze frame event when played, pausing the game for the specified duration (usually short, but not necessarily)
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will freeze the timescale for the specified duration (in seconds). I usually go with 0.01s or 0.02s, but feel free to tweak it to your liking. It requires a MMTimeManager in your scene to work.")]
    [FeedbackPath("Time/Freeze Frame")]
    public class MMFeedbackFreezeFrame : MMFeedback
    {
        [Header("Freeze Frame")]
        /// the duration of the freeze frame
        public float FreezeFrameDuration = 0.02f;

        /// <summary>
        /// On Play we trigger a freeze frame event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMFreezeFrameEvent.Trigger(FreezeFrameDuration);
            }
        }
    }
}
