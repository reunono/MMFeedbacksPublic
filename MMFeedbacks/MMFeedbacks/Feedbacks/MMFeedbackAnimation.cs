using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A feedback used to trigger an animation (bool or trigger) on the associated animator
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will allow you to send to an animator (bound in its inspector) a bool or trigger parameter, allowing you to trigger an animation.")]
    [FeedbackPath("GameObject/Animation")]
    public class MMFeedbackAnimation : MMFeedback
    {
        [Header("Animation")]
        /// the animator whose parameters you want to update
        public Animator BoundAnimator;
        /// the trigger animator parameter to well, trigger when the feedback is played
        public string TriggerParameterName;
        /// the bool parameter to turn true when the feedback gets played
        public string BoolParameterName;
        
        /// <summary>
        /// Custom Init
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
        }

        /// <summary>
        /// On Play, checks if an animator is bound and triggers parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                if (BoundAnimator == null)
                {
                    Debug.LogWarning("No animator was set for " + this);
                    return;
                }

                if (TriggerParameterName != "")
                {
                    BoundAnimator.SetTrigger(TriggerParameterName);
                }

                if (BoolParameterName != "")
                {
                    BoundAnimator.SetBool(BoolParameterName, true);
                }
            }
        }
        
        /// <summary>
        /// On stop, turns the bool parameter to false
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (BoolParameterName != ""))
            {
                BoundAnimator.SetBool(BoolParameterName, false);
            }
        }
    }
}
