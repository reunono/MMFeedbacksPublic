using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will trigger a flash event (to be caught by a MMFlash) when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("On play, this feedback will broadcast a MMFlashEvent. If you create a UI image with a MMFlash component on it (see example in the Demo scene), it will intercept that event, and flash (usually you'll want it to take the full size of your screen, but that's not mandatory). In the feedback's inspector, you can define the color of the flash, its duration, alpha, and a FlashID. That FlashID needs to be the same on your feedback and MMFlash for them to work together. This allows you to have multiple MMFlashs in your scene, and flash them separately.")]
    [FeedbackPath("Camera/Flash")]
    public class MMFeedbackFlash : MMFeedback
    {
        [Header("Flash")]
        /// the channel to broadcast that flash event on
        public int Channel = 0;
        /// the color of the flash
        public Color FlashColor = Color.white;
        /// the flash duration (in seconds)
        public float FlashDuration = 0.2f;
        /// the alpha of the flash
        public float FlashAlpha = 1f;
        /// the ID of the flash (usually 0). You can specify on each MMFlash object an ID, allowing you to have different flash images in one scene and call them separately (one for damage, one for health pickups, etc)
        public int FlashID = 0;

        /// <summary>
        /// On Play we trigger a flash event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMFlashEvent.Trigger(FlashColor, FlashDuration * attenuation, FlashAlpha, FlashID, Channel);
            }
        }
    }
}
