using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This class will allow you to trigger zooms on your camera by sending MMCameraZoomEvents from any other class
    /// </summary>

    [RequireComponent(typeof(Camera))]
    public class MMCameraZoom : MonoBehaviour
    {
        public int Channel = 0;
        [Header("Transition Speed")]
        /// the animation curve to apply to the zoom transition
        public AnimationCurve ZoomCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [Header("Test Zoom")]
        /// the mode to apply the zoom in when using the test button in the inspector
        public MMCameraZoomModes TestMode;
        /// the target field of view to apply the zoom in when using the test button in the inspector
        public float TestFieldOfView = 30f;
        /// the transition duration to apply the zoom in when using the test button in the inspector
        public float TestTransitionDuration = 0.1f;
        /// the duration to apply the zoom in when using the test button in the inspector
        public float TestDuration = 0.05f;

        [MMFInspectorButton("TestZoom")]
        /// an inspector button to test the zoom in play mode
        public bool TestZoomButton;

        protected Camera _camera;
        protected float _initialFieldOfView;
        protected MMCameraZoomModes _mode;
        protected bool _zooming = false;
        protected float _startFieldOfView;
        protected float _transitionDuration;
        protected float _duration;
        protected float _targetFieldOfView;
        protected float _delta = 0f;
        protected int _direction = 1;
        protected float _reachedDestinationTimestamp;
        protected bool _destinationReached = false;

        /// <summary>
        /// On Awake we grab our virtual camera
        /// </summary>
        protected virtual void Awake()
        {
            _camera = this.gameObject.GetComponent<Camera>();
            _initialFieldOfView = _camera.fieldOfView;
        }	
        
        /// <summary>
        /// On Update if we're zooming we modify our field of view accordingly
        /// </summary>
        protected virtual void Update()
        {
            if (!_zooming)
            {
                return;
            }

            if (_camera.fieldOfView != _targetFieldOfView)
            {
                _delta += Time.deltaTime / _transitionDuration;
                _camera.fieldOfView = Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, ZoomCurve.Evaluate(_delta));
            }
            else
            {
                if (!_destinationReached)
                {
                    _reachedDestinationTimestamp = Time.time;
                    _destinationReached = true;
                }

                if ((_mode == MMCameraZoomModes.For) && (_direction == 1))
                {
                    if (Time.time - _reachedDestinationTimestamp > _duration)
                    {
                        _direction = -1;
                        _startFieldOfView = _targetFieldOfView;
                        _targetFieldOfView = _initialFieldOfView;
                        _delta = 0f;
                    }                    
                }
                else
                {
                    _zooming = false;
                }                
            }
        }

        /// <summary>
        /// A method that triggers the zoom, ideally only to be called via an event, but public for convenience
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="newFieldOfView"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="duration"></param>
        public virtual void Zoom(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration)
        {
            if (_zooming)
            {
                return;
            }

            _zooming = true;
            _delta = 0f;
            _mode = mode;

            _startFieldOfView = _camera.fieldOfView;
            _transitionDuration = transitionDuration;
            _duration = duration;
            _transitionDuration = transitionDuration;
            _direction = 1;
            _destinationReached = false;

            switch (mode)
            {
                case MMCameraZoomModes.For:
                    _targetFieldOfView = newFieldOfView;
                    break;

                case MMCameraZoomModes.Set:
                    _targetFieldOfView = newFieldOfView;
                    break;

                case MMCameraZoomModes.Reset:
                    _targetFieldOfView = _initialFieldOfView;
                    break;
            }

        }

        /// <summary>
        /// The method used by the test button to trigger a test zoom
        /// </summary>
        protected virtual void TestZoom()
        {
            Zoom(TestMode, TestFieldOfView, TestTransitionDuration, TestDuration);
        }

        /// <summary>
        /// When we get an MMCameraZoomEvent we call our zoom method 
        /// </summary>
        /// <param name="zoomEvent"></param>
        public virtual void OnCameraZoomEvent(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            this.Zoom(mode, newFieldOfView, transitionDuration, duration);
        }

        /// <summary>
        /// Starts listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnEnable()
        {
            MMCameraZoomEvent.Register(OnCameraZoomEvent);
        }

        /// <summary>
        /// Stops listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnDisable()
        {
            MMCameraZoomEvent.Unregister(OnCameraZoomEvent);
        }
    }
}
