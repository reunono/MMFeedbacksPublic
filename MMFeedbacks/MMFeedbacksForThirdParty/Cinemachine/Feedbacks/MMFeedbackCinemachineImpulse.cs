using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Cinemachine;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("Cinemachine/Impulse")]
    [FeedbackHelp("This feedback lets you trigger a Cinemachine Impulse event. You'll need a Cinemachine Impulse Listener on your camera for this to work.")]
    public class MMFeedbackCinemachineImpulse : MMFeedback
    {
        [Header("Cinemachine Impulse")]
        [CinemachineImpulseDefinitionProperty]
        public CinemachineImpulseDefinition m_ImpulseDefinition;
        public Vector3 Velocity;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                m_ImpulseDefinition.CreateEvent(position, Velocity);
            }
        }
    }
}
