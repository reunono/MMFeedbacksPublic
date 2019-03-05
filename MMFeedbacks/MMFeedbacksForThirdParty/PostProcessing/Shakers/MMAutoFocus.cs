using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This class will set the depth of field to focus on the set of targets specified in its inspector.
    /// </summary>
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMAutoFocus : MonoBehaviour
    {
        // Array of targets
        public Transform[] FocusTargets;

        // Current target
        public float FocusTargetID;

        // Cache profile
        PostProcessVolume _volume;
        PostProcessProfile _profile;
        DepthOfField _depthOfField;

        [Range(0.1f, 20f)] public float Aperture;


        void Start()
        {
            _volume = GetComponent<PostProcessVolume>();
            _profile = _volume.profile;
            _profile.TryGetSettings<DepthOfField>(out _depthOfField);
        }

        void Update()
        {

            // Set variables
            // Get distance from camera and target
            float distance = Vector3.Distance(transform.position, FocusTargets[Mathf.FloorToInt(FocusTargetID)].position);
            _depthOfField.focusDistance.Override(distance);
            _depthOfField.aperture.Override(Aperture);
        }
    }
}
