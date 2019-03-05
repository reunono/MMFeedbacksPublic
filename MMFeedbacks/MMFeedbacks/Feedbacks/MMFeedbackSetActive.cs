using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Turns an object active or inactive at the various stages of the feedback
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to change the state of the target gameobject from active to inactive (or the opposite), on init, play, stop or reset. For each of these you can specify if you want to force a state (active or inactive), or toggle it (active becomes inactive, inactive becomes active).")]
    [FeedbackPath("GameObject/Set Active")]
    public class MMFeedbackSetActive : MMFeedback
    {
        /// the possible effects the feedback can have on the target object's status 
        public enum PossibleStates { Active, Inactive, Toggle }
        
        [Header("Set Active")]
        /// the gameobject we want to change the active state of
        public GameObject TargetGameObject;
        /// whether or not we should alter the state of the target object on init
        public bool SetStateOnInit = false;
        [MMFCondition("SetStateOnInit", true)]
        /// how to change the state on init
        public PossibleStates StateOnInit = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on play
        public bool SetStateOnPlay = false;
        [MMFCondition("SetStateOnPlay", true)]
        /// how to change the state on play
        public PossibleStates StateOnPlay = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on stop
        public bool SetStateOnStop = false;
        [MMFCondition("SetStateOnStop", true)]
        /// how to change the state on stop
        public PossibleStates StateOnStop = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on reset
        public bool SetStateOnReset = false;
        [MMFCondition("SetStateOnReset", true)]
        /// how to change the state on reset
        public PossibleStates StateOnReset = PossibleStates.Inactive;
        
        /// <summary>
        /// On init we change the state of our object if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active && (TargetGameObject != null))
            {
                if (SetStateOnInit)
                {
                    SetStatus(StateOnInit);
                }
            }
        }

        /// <summary>
        /// On Play we change the state of our object if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (TargetGameObject != null))
            {
                if (SetStateOnPlay)
                {
                    SetStatus(StateOnPlay);
                }
            }
        }

        /// <summary>
        /// On Stop we change the state of our object if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);

            if (Active && (TargetGameObject != null))
            {
                if (SetStateOnStop)
                {
                    SetStatus(StateOnStop);
                }
            }
        }

        /// <summary>
        /// On Reset we change the state of our object if needed
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();

            if (Active && (TargetGameObject != null))
            {
                if (SetStateOnReset)
                {
                    SetStatus(StateOnReset);
                }
            }
        }

        /// <summary>
        /// Changes the status of the object
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetStatus(PossibleStates state)
        {
            switch (state)
            {
                case PossibleStates.Active:
                    TargetGameObject.SetActive(true);
                    break;
                case PossibleStates.Inactive:
                    TargetGameObject.SetActive(false);
                    break;
                case PossibleStates.Toggle:
                    TargetGameObject.SetActive(!TargetGameObject.activeInHierarchy);
                    break;
            }
        }
    }
}
