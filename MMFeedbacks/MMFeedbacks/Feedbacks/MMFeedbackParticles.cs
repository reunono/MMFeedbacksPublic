using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will play the associated particles system on play, and stop it on stop
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will simply play the specified ParticleSystem (from your scene) when played.")]
    [FeedbackPath("Particles/Particles Play")]
    public class MMFeedbackParticles : MMFeedback
    {
        [Header("Bound Particles")]
        /// the particle system to play with this feedback
        public ParticleSystem BoundParticleSystem; 

        /// <summary>
        /// On init we stop our particle system
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            BoundParticleSystem?.Stop();
        }

        /// <summary>
        /// On play we play our particle system
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }
            BoundParticleSystem?.Play();
        }
        
        /// <summary>
        /// On Stop, stops the particle system
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            BoundParticleSystem?.Stop();
        }

        /// <summary>
        /// On Reset, stops the particle system 
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            BoundParticleSystem?.Stop();
        }
    }
}
