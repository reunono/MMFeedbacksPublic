using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
#if NICEVIBRATIONS_INSTALLED
using MoreMountains.NiceVibrations;
#endif

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("Haptics")]
    [FeedbackHelp("This feedback lets you trigger haptic feedbacks through the Nice Vibrations asset, available on the Unity Asset Store. You'll need to own that asset and have it " +
        "in your project for this to work.")]
    public class MMFeedbackHaptics : MMFeedback
    {
#if NICEVIBRATIONS_INSTALLED
        [Header("Haptics")]
        public HapticTypes HapticType = HapticTypes.None;
#endif

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
#if NICEVIBRATIONS_INSTALLED
                MMVibrationManager.Haptic(HapticType);
#endif
            }
        }
    }
}
