using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace MoreMountains.Feedbacks
{

    public struct MMFlashEvent
    {
        public delegate void Delegate(Color flashColor, float duration, float alpha, int flashID, int channel);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(Color flashColor, float duration, float alpha, int flashID, int channel)
        {
            OnEvent?.Invoke(flashColor, duration, alpha, flashID, channel);
        }
    }
    
	[RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// Add this class to an image and it'll flash when getting a MMFlashEvent
    /// </summary>
    public class MMFlash : MonoBehaviour
    {
        /// the channel to receive flash events on
        public int Channel = 0;
        /// the ID of this MMFlash object. When triggering a MMFlashEvent you can specify an ID, and only MMFlash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene
        public int FlashID = 0;

		protected Image _image;
        protected CanvasGroup _canvasGroup;
		protected bool _flashing = false;
        protected float _targetAlpha;
        protected Color _initialColor;
        protected float _delta;
        protected float _flashStartedTimestamp;
        protected int _direction = 1;
        protected float _duration;        

		/// <summary>
		/// On start we grab our image component
		/// </summary>
		protected virtual void Start()
		{
			_image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialColor = _image.color;

        }

		/// <summary>
		/// On update we flash our image if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_flashing)
			{
				_image.enabled = true;


                if (Time.time - _flashStartedTimestamp > _duration / 2f)
                {
                    _direction = -1;
                }

                if (_direction == 1)
                {
                    _delta += Time.deltaTime / (_duration / 2f);
                }
                else
                {
                    _delta -= Time.deltaTime / (_duration / 2f);
                }
                
                if (Time.time - _flashStartedTimestamp > _duration)
                {
                    _flashing = false;
                }

                _canvasGroup.alpha = Mathf.Lerp(0f, _targetAlpha, _delta);
            }
			else
			{
				_image.enabled = false;
			}
		}

		/// <summary>
		/// When getting a flash event, we turn our image on
		/// </summary>
		public void OnMMFlashEvent(Color flashColor, float duration, float alpha, int flashID, int channel)
        {
            if (flashID != FlashID) 
            {
                return;
            }

            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            if (!_flashing)
            {
                _flashing = true;
                _direction = 1;
                _canvasGroup.alpha = 0;
                _targetAlpha = alpha;
                _delta = 0f;
                _image.color = flashColor;
                _duration = duration;
                _flashStartedTimestamp = Time.time;
            }
        } 

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
            MMFlashEvent.Register(OnMMFlashEvent);
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
            MMFlashEvent.Unregister(OnMMFlashEvent);
        }		
	}
}