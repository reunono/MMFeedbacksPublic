using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A feedback that will allow you to change the zoom of a (3D) camera when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("Define zoom properties : For will set the zoom to the specified parameters for a certain duration, Set will leave them like that forever. Zoom properties include the field of view, the duration of the zoom transition (in seconds) and the zoom duration (the time the camera should remain zoomed in, in seconds). For this to work, you'll need to add a MMCameraZoom component to your Camera.")]
    [FeedbackPath("Camera/Camera Zoom")]
    public class MMFeedbackCameraZoom : MMFeedback
    {
        [Header("Camera Zoom")]
        /// the channel to broadcast that zoom event on
        public int Channel = 0;
        /// the zoom mode (for : forward for TransitionDuration, static for Duration, backwards for TransitionDuration)
        public MMCameraZoomModes ZoomMode = MMCameraZoomModes.For;
        /// the target field of view
        public float ZoomFieldOfView = 30f;
        /// the zoom transition duration
        public float ZoomTransitionDuration = 0.05f;
        /// the duration for which the zoom is at max zoom
        public float ZoomDuration = 0.1f;

        /// <summary>
        /// On Play, triggers a zoom event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMCameraZoomEvent.Trigger(ZoomMode, ZoomFieldOfView, ZoomTransitionDuration, ZoomDuration, Channel);
            }
        }
    }
}
