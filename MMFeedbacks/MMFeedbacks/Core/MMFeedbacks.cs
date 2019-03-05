using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A collection of MMFeedback, meant to be played altogether.
    /// This class provides a custom inspector to add and customize feedbacks, and public methods to trigger them, stop them, etc.
    /// You can either use it on its own, or bind it from another class and trigger it from there.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class MMFeedbacks : MonoBehaviour
    {
        /// a list of MMFeedback to trigger
        public List<MMFeedback> Feedbacks = new List<MMFeedback>();
        /// the possible initialization modes. If you use Script, you'll have to initialize manually by calling the Initialization method and passing it an owner
        /// Otherwise, you can have this component initialize itself at Awake or Start, and in this case the owner will be the MMFeedbacks itself
        public enum InitializationModes { Script, Awake, Start }
        /// the chosen initialization mode
        public InitializationModes InitializationMode = InitializationModes.Start;
        [HideInInspector]
        /// whether or not this MMFeedbacks is in debug mode
        public bool DebugActive = false;

        /// <summary>
        /// On Awake we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected virtual void Awake()
        {
            if ((InitializationMode == InitializationModes.Awake) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// On Start we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected virtual void Start()
        {
            if ((InitializationMode == InitializationModes.Start) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// Initializes the MMFeedbacks, setting this MMFeedbacks as the owner
        /// </summary>
        public virtual void Initialization()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {

                Feedbacks[i].Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// A public method to initialize the feedback, specifying an owner that will be used as the reference for position and hierarchy by feedbacks
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="feedbacksOwner"></param>
        public virtual void Initialization(GameObject owner)
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {

                Feedbacks[i].Initialization(owner);
            }
        }

        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation
        /// </summary>
        public virtual void PlayFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Play(this.transform.position, 1.0f);
            }
        }

        /// <summary>
        /// Plays all feedbacks, specifying a position and attenuation. The position may be used by each Feedback and taken into account to spark a particle or play a sound for example.
        /// The attenuation is a factor that can be used by each Feedback to lower its intensity, usually you'll want to define that attenuation based on time or distance (using a lower 
        /// attenuation value for feedbacks happening further away from the Player).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksOwner"></param>
        /// <param name="attenuation"></param>
        public virtual void PlayFeedbacks(Vector3 position, float attenuation = 1.0f)
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Play(position, attenuation);
            }
        }

        /// <summary>
        /// Stops all feedbacks from playing. 
        /// </summary>
        public virtual void StopFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Stop(this.transform.position, 1.0f);
            }
        }

        /// <summary>
        /// Stops all feedbacks from playing, specifying a position and attenuation that can be used by the Feedbacks 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void StopFeedbacks(Vector3 position, float attenuation = 1.0f)
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Stop(position, attenuation);
            }
        }

        /// <summary>
        /// Calls each feedback's Reset method if they've defined one. An example of that can be resetting the initial color of a flickering renderer.
        /// </summary>
        public virtual void ResetFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].ResetFeedback();
            }
        }

        /// <summary>
        /// On Destroy, removes all feedbacks from this MMFeedbacks to avoid any leftovers
        /// </summary>
        protected virtual void OnDestroy()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {            
                // we remove all binders
                foreach (MMFeedback feedback in Feedbacks)
                {
                    EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(feedback);
                    };                    
                }
            }
            #endif
        }
            
    }

}
