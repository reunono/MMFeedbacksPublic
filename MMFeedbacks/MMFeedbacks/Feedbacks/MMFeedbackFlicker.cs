using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will make the bound renderer flicker for the set duration when played (and restore its initial color when stopped)
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you flicker the specified renderer (sprite, mesh, etc) for a certain duration, at the specified octave, and with the specified color. Useful when a character gets hit, for example (but so much more!).")]
    [FeedbackPath("GameObject/Flicker")]
    public class MMFeedbackFlicker : MMFeedback
    {
        [Header("Flicker")]
        /// the renderer to flicker when played
        public Renderer BoundRenderer;
        /// the duration of the flicker when getting damage
        public float FlickerDuration = 0.2f;
        /// the frequency at which to flicker
        public float FlickerOctave = 0.04f;
        /// the color we should flicker the sprite to 
        public Color FlickerColor = new Color32(255, 20, 20, 255);

        protected Color _initialFlickerColor;

        /// <summary>
        /// On init we grab our initial color and components
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            if (Active && (BoundRenderer != null))
            {
                if (BoundRenderer.material.HasProperty("_Color"))
                {
                    _initialFlickerColor = BoundRenderer.material.color;
                }
            }
            if (Active && (BoundRenderer == null) && (owner != null))
            {
                if (owner.MMFGetComponentNoAlloc<Renderer>() != null)
                {
                    BoundRenderer = owner.GetComponent<Renderer>();
                }
                if (BoundRenderer == null)
                {
                    BoundRenderer = owner.GetComponentInChildren<Renderer>();
                }
                if (BoundRenderer != null)
                {
                    if (BoundRenderer.material.HasProperty("_Color"))
                    {
                        _initialFlickerColor = BoundRenderer.material.color;
                    }
                }
            }
        }

        /// <summary>
        /// On play we make our renderer flicker
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (BoundRenderer != null))
            {
                StartCoroutine(Flicker(BoundRenderer, _initialFlickerColor, FlickerColor, FlickerOctave, FlickerDuration));
            }
        }

        /// <summary>
        /// On reset we make our renderer stop flickering
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            if (Active && (BoundRenderer != null))
            {
                if (BoundRenderer.material.HasProperty("_Color"))
                {
                    BoundRenderer.material.color = _initialFlickerColor;
                }
            }
        }

        public virtual IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            if (!renderer.material.HasProperty("_Color"))
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;

            while (Time.time < flickerStop)
            {
                renderer.material.color = flickerColor;
                yield return new WaitForSeconds(flickerSpeed);
                renderer.material.color = initialColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            renderer.material.color = initialColor;
        }
    }
}
