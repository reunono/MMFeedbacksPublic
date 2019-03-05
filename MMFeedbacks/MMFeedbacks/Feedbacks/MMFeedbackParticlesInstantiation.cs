using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will instantiate a particle system and play/stop it when playing/stopping the feedback
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will instantiate the specified ParticleSystem at the specified position on Start, optionally nesting them.")]
    [FeedbackPath("Particles/Particles Instantiation")]
    public class MMFeedbackParticlesInstantiation : MMFeedback
    {
        /// the particle system to spawn
        public ParticleSystem ParticlesPrefab;
        /// the position at which to spawn this particle system
        public Transform InstantiateParticlesPosition;
        /// whether or not the particle system should be nested in hierarchy or floating on its own
        public bool NestParticles = true;

        protected ParticleSystem _instantiatedParticleSystem;

        /// <summary>
        /// On init, instantiates the particle system, positions it and nests it if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            if (Active)
            {
                if (_instantiatedParticleSystem != null)
                {
                    Destroy(_instantiatedParticleSystem.gameObject);
                }

                _instantiatedParticleSystem = GameObject.Instantiate(ParticlesPrefab) as ParticleSystem;
                _instantiatedParticleSystem.Stop();

                if (InstantiateParticlesPosition == null)
                {
                    if (Owner != null)
                    {
                        InstantiateParticlesPosition = Owner.transform;
                    }
                }

                if (InstantiateParticlesPosition)
                {
                    _instantiatedParticleSystem.gameObject.transform.position = InstantiateParticlesPosition.transform.position;
                    if (NestParticles)
                    {
                        _instantiatedParticleSystem.transform.SetParent(InstantiateParticlesPosition);
                    }                    
                }
                else
                {
                    _instantiatedParticleSystem.gameObject.transform.position = this.transform.position;
                    if (NestParticles)
                    {
                        this.transform.SetParent(InstantiateParticlesPosition);
                    }                    
                }
            }
        }

        /// <summary>
        /// On Play, plays the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            _instantiatedParticleSystem?.Play();
        }

        /// <summary>
        /// On Stop, stops the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            _instantiatedParticleSystem?.Stop();
        }

        /// <summary>
        /// On Reset, stops the feedback
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();

            if (!Active)
            {
                return;
            }

            _instantiatedParticleSystem?.Stop();
        }
    }
}
