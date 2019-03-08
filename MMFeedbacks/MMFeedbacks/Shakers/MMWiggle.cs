using UnityEngine;
using System.Collections;
using System;

namespace MoreMountains.Feedbacks
{
    /// the possible types of wiggle
    public enum WiggleTypes { None, Random, PingPong, Noise }

    /// <summary>
    /// A class to store public wiggle properties
    /// </summary>
    [Serializable]
    public class WiggleProperties
    {
        [Header("Status")]
        public bool WigglePermitted = true;

        [Header("Type")]
        /// the position mode : none, random or ping pong - none won't do anything, random will randomize min and max bounds, ping pong will oscillate between min and max bounds
        public WiggleTypes WiggleType = WiggleTypes.Random;
        /// if this is true, unscaled delta time, otherwise regular delta time
        public bool UseUnscaledTime = false;
        /// whether or not this object should start wiggling automatically on Start()
        public bool StartWigglingAutomatically = true;
        /// if this is true, position will be ping ponged with an ease in/out curve
        public bool SmoothPingPong = true;

        [Header("Speed")]
        /// Whether or not the position's speed curve will be used
        public bool UseSpeedCurve = false;
        /// an animation curve to define the speed over time from one position to the other (x), and the actual position (y), allowing for overshoot
        public AnimationCurve SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Frequency")]
        /// the minimum time (in seconds) between two position changes
        public float FrequencyMin = 0f;
        /// the maximum time (in seconds) between two position changes
        public float FrequencyMax = 1f;

        [Header("Amplitude")]
        /// the minimum position the object can have
        public Vector3 AmplitudeMin = Vector3.zero;
        /// the maximum position the object can have
        public Vector3 AmplitudeMax = Vector3.one;
        /// if this is true, amplitude will be relative, otherwise world space
        public bool RelativeAmplitude = true;

        [Header("Pause")]
        /// the minimum time to spend between two random positions
        public float PauseMin = 0f;
        /// the maximum time to spend between two random positions
        public float PauseMax = 0f;

        [Header("Limited Time")]
        /// if this is true, this property will only animate for the specified time
        public bool LimitedTime = false;
        /// the maximum time left
        public float LimitedTimeTotal;
        /// the animation curve to use to decrease the effect of the wiggle as time goes
        public AnimationCurve LimitedTimeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        /// if this is true, original position will be restored when time left reaches zero
        public bool LimitedTimeResetValue = true;
        /// the actual time left
        [MMFReadOnly]
        public float LimitedTimeLeft;        

        [Header("Noise Frequency")]
        /// the minimum time between two changes of noise frequency
        public Vector3 NoiseFrequencyMin = Vector3.zero;
        /// the maximum time between two changes of noise frequency
        public Vector3 NoiseFrequencyMax = Vector3.one;

        [Header("Noise Shift")]
        /// how much the noise should be shifted at minimum
        public Vector3 NoiseShiftMin = Vector3.zero;
        /// how much the noise should be shifted at maximum
        public Vector3 NoiseShiftMax = Vector3.zero;


        /// <summary>
        /// Returns the delta time, either regular or unscaled
        /// </summary>
        /// <returns></returns>
        public float GetDeltaTime()
        {
            return UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        /// <summary>
        /// Returns the time, either regular or unscaled
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return UseUnscaledTime ? Time.unscaledTime : Time.time;
        }
    }

    /// <summary>
    /// A struct used to store internal wiggle properties
    /// </summary>
    public struct InternalWiggleProperties
    {
        public Vector3 returnVector;
        public Vector3 newValue;
        public Vector3 initialValue;
        public Vector3 startValue;
        public float timeSinceLastChange ;
        public float randomFrequency;
        public Vector3 randomNoiseFrequency;
        public Vector3 randomAmplitude;
        public Vector3 randomNoiseShift;
        public float timeSinceLastPause;
        public float pauseDuration;
        public float noiseElapsedTime;
        public Vector3 limitedTimeValueSave;
    }

    /// <summary>
    /// Add this class to a GameObject to be able to control its position/rotation/scale individually and periodically, allowing it to "wiggle" (or just move however you want on a periodic basis)
    /// </summary>
    public class MMWiggle : MonoBehaviour 
    {
        /// whether or not position wiggle is active
        public bool PositionActive = false;
        /// whether or not rotation wiggle is active
        public bool RotationActive = false;
        /// whether or not scale wiggle is active
        public bool ScaleActive = false;
        /// all public info related to position wiggling
        public WiggleProperties PositionWiggleProperties;
        /// all public info related to rotation wiggling
        public WiggleProperties RotationWiggleProperties;
        /// all public info related to scale wiggling
        public WiggleProperties ScaleWiggleProperties;
        /// a debug duration used in conjunction with the debug buttons
        public float DebugWiggleDuration = 2f;

        protected InternalWiggleProperties _positionInternalProperties;
        protected InternalWiggleProperties _rotationInternalProperties;
        protected InternalWiggleProperties _scaleInternalProperties;

        public virtual void WigglePosition(float duration)
        {
            WiggleValue(ref PositionWiggleProperties, ref _positionInternalProperties, duration);
        }

        public virtual void WiggleRotation(float duration)
        {
            WiggleValue(ref RotationWiggleProperties, ref _rotationInternalProperties, duration);
        }

        public virtual void WiggleScale(float duration)
        {
            WiggleValue(ref ScaleWiggleProperties, ref _scaleInternalProperties, duration);
        }

        protected virtual void WiggleValue(ref WiggleProperties property, ref InternalWiggleProperties internalProperties, float duration)
        {
            InitializeRandomValues(ref property, ref internalProperties);
            internalProperties.limitedTimeValueSave = internalProperties.initialValue;
            property.LimitedTime = true;
            property.LimitedTimeLeft = duration;
            property.LimitedTimeTotal = duration;
            property.WigglePermitted = true;
        }

        /// <summary>
        /// On Start() we trigger the initialization
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        /// On init we get the start values and trigger our coroutines for each property
        /// </summary>
        protected virtual void Initialization()
        {
            _positionInternalProperties.initialValue = transform.localPosition;
            _positionInternalProperties.startValue = this.transform.localPosition;

            _rotationInternalProperties.initialValue = transform.localEulerAngles;
            _rotationInternalProperties.startValue = this.transform.localEulerAngles;

            _scaleInternalProperties.initialValue = transform.localScale;
            _scaleInternalProperties.startValue = this.transform.localScale;

            InitializeRandomValues(ref PositionWiggleProperties, ref _positionInternalProperties);
            InitializeRandomValues(ref RotationWiggleProperties, ref _rotationInternalProperties);
            InitializeRandomValues(ref ScaleWiggleProperties, ref _scaleInternalProperties);
        }

        /// <summary>
        /// Initializes internal properties of the specified wiggle value
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="internalProperties"></param>
        protected virtual void InitializeRandomValues(ref WiggleProperties properties, ref InternalWiggleProperties internalProperties)
        {
            internalProperties.newValue = Vector3.zero;
            internalProperties.timeSinceLastChange = 0;
            internalProperties.returnVector = Vector3.zero;
            internalProperties.randomFrequency = 0f;
            internalProperties.randomNoiseFrequency = Vector3.zero;
            internalProperties.randomAmplitude = Vector3.zero;
            internalProperties.timeSinceLastPause = 0;
            internalProperties.pauseDuration = 0;
            internalProperties.noiseElapsedTime = 0;
            properties.LimitedTimeLeft = properties.LimitedTimeTotal;

            RandomizeVector3(ref internalProperties.randomAmplitude, properties.AmplitudeMin, properties.AmplitudeMax);
            RandomizeVector3(ref internalProperties.randomNoiseFrequency, properties.NoiseFrequencyMin, properties.NoiseFrequencyMax);
            RandomizeVector3(ref internalProperties.randomNoiseShift, properties.NoiseShiftMin, properties.NoiseShiftMax);

            internalProperties.newValue = DetermineNewValue(properties, internalProperties.newValue, internalProperties.initialValue, ref internalProperties.startValue, 
                ref internalProperties.randomAmplitude, ref internalProperties.randomFrequency, ref internalProperties.pauseDuration);
        }
        
        /// <summary>
        /// Every frame we update our object's position, rotation and scale
        /// </summary>
        protected virtual void Update()
        {
            _positionInternalProperties.returnVector = transform.localPosition;
            if (UpdateValue(PositionActive, PositionWiggleProperties, ref _positionInternalProperties))
            {
                transform.localPosition = ApplyFalloff(_positionInternalProperties.returnVector, PositionWiggleProperties);
            }

            _rotationInternalProperties.returnVector = transform.localEulerAngles;
            if (UpdateValue(RotationActive, RotationWiggleProperties, ref _rotationInternalProperties))
            {
                transform.localEulerAngles = ApplyFalloff(_rotationInternalProperties.returnVector, RotationWiggleProperties);
            }

            _scaleInternalProperties.returnVector = transform.localScale;
            if (UpdateValue(ScaleActive, ScaleWiggleProperties, ref _scaleInternalProperties))
            {
                transform.localScale = ApplyFalloff(_scaleInternalProperties.returnVector, ScaleWiggleProperties);
            }
        }

        /// <summary>
        /// Computes the next Vector3 value for the specified property
        /// </summary>
        /// <param name="valueActive"></param>
        /// <param name="properties"></param>
        /// <param name="internalProperties"></param>
        /// <returns></returns>
        protected virtual bool UpdateValue(bool valueActive, WiggleProperties properties, ref InternalWiggleProperties internalProperties)
        {
            if (!valueActive) { return false; }
            if (!properties.WigglePermitted) { return false;  }

            // handle limited time
            if ((properties.LimitedTime) && (properties.LimitedTimeTotal > 0f))
            {
                float timeSave = properties.LimitedTimeLeft;
                properties.LimitedTimeLeft -= properties.GetDeltaTime();
                if (properties.LimitedTimeLeft <= 0)
                {
                    if (timeSave > 0f)
                    {
                        if (properties.LimitedTimeResetValue)
                        {
                            internalProperties.returnVector = internalProperties.limitedTimeValueSave;
                            properties.LimitedTimeLeft = 0;
                            properties.WigglePermitted = false;
                            return true;
                        }
                    }                    
                    return false;
                }
            }

            switch (properties.WiggleType)
            {
                case WiggleTypes.PingPong:
                    return MoveVector3TowardsTarget(ref internalProperties.returnVector, properties, ref internalProperties.startValue, internalProperties.initialValue, 
                        ref internalProperties.newValue, ref internalProperties.timeSinceLastPause, 
                        ref internalProperties.timeSinceLastChange, ref internalProperties.randomAmplitude, 
                        ref internalProperties.randomFrequency, 
                        ref internalProperties.pauseDuration, internalProperties.randomFrequency);
                    

                case WiggleTypes.Random:
                    return MoveVector3TowardsTarget(ref internalProperties.returnVector, properties, ref internalProperties.startValue, internalProperties.initialValue, 
                        ref internalProperties.newValue, ref internalProperties.timeSinceLastPause, 
                        ref internalProperties.timeSinceLastChange, ref internalProperties.randomAmplitude, 
                        ref internalProperties.randomFrequency, 
                        ref internalProperties.pauseDuration, internalProperties.randomFrequency);

                case WiggleTypes.Noise:
                    internalProperties.returnVector = AnimateNoiseValue(ref internalProperties, properties);                    
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Applies a falloff to the computed value based on time spent and a falloff animation curve
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected Vector3 ApplyFalloff(Vector3 newValue, WiggleProperties properties)
        {
            if ((properties.LimitedTime) && (properties.LimitedTimeTotal > 0f))
            {
                float curveProgress = (properties.LimitedTimeTotal - properties.LimitedTimeLeft) / properties.LimitedTimeTotal;
                float curvePercent = properties.LimitedTimeFalloff.Evaluate(curveProgress);
                newValue = newValue * curvePercent; //Vector3.LerpUnclamped(startValue, destinationValue, curvePercent);
            }
            return newValue;
        }

        /// <summary>
        /// Animates a Vector3 value along a perlin noise
        /// </summary>
        /// <param name="internalProperties"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected virtual Vector3 AnimateNoiseValue(ref InternalWiggleProperties internalProperties, WiggleProperties properties)
        {
            internalProperties.noiseElapsedTime += properties.GetDeltaTime();

            internalProperties.newValue.x = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.x * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.x) * 2.0f - 1.0f) * internalProperties.randomAmplitude.x;
            internalProperties.newValue.y = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.y * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.y) * 2.0f - 1.0f) * internalProperties.randomAmplitude.y;
            internalProperties.newValue.z = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.z * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.z) * 2.0f - 1.0f) * internalProperties.randomAmplitude.z;
            
            if (properties.RelativeAmplitude)
            {
                internalProperties.newValue += internalProperties.initialValue;
            }
            
            return internalProperties.newValue;
        }

        /// <summary>
        /// Moves a vector3's values towards a target
        /// </summary>
        /// <param name="movedValue"></param>
        /// <param name="properties"></param>
        /// <param name="startValue"></param>
        /// <param name="initialValue"></param>
        /// <param name="destinationValue"></param>
        /// <param name="timeSinceLastPause"></param>
        /// <param name="timeSinceLastValueChange"></param>
        /// <param name="randomAmplitude"></param>
        /// <param name="randomFrequency"></param>
        /// <param name="pauseDuration"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        protected virtual bool MoveVector3TowardsTarget(ref Vector3 movedValue, WiggleProperties properties, ref Vector3 startValue, Vector3 initialValue, 
            ref Vector3 destinationValue, ref float timeSinceLastPause, ref float timeSinceLastValueChange, 
            ref Vector3 randomAmplitude, ref float randomFrequency,
            ref float pauseDuration, float frequency)
        {
            timeSinceLastPause += properties.GetDeltaTime();
            timeSinceLastValueChange += properties.GetDeltaTime();

            // handle pause
            if (timeSinceLastPause < pauseDuration)
            {
                return false;
            }
            
            // if we're just out of a pause
            if (timeSinceLastPause == timeSinceLastValueChange)
            {
                timeSinceLastValueChange = 0f;
            }

            // if we've reached the end
            if (frequency > 0)
            {
                float curveProgress = (timeSinceLastValueChange) / frequency;

                if (!properties.UseSpeedCurve)
                {
                    movedValue = Vector3.Lerp(startValue, destinationValue, curveProgress);
                }
                else
                {
                    float curvePercent = properties.SpeedCurve.Evaluate(curveProgress);
                    movedValue = Vector3.LerpUnclamped(startValue, destinationValue, curvePercent);
                }


                if (timeSinceLastValueChange > frequency)
                {
                    timeSinceLastValueChange = 0f;
                    timeSinceLastPause = 0f;
                    movedValue = destinationValue;
                    destinationValue = DetermineNewValue(properties, movedValue, initialValue, ref startValue, 
                        ref randomAmplitude, ref randomFrequency, ref pauseDuration);
                }
            }
            return true;
        }

        /// <summary>
        /// Picks a new target value
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="newValue"></param>
        /// <param name="initialValue"></param>
        /// <param name="startValue"></param>
        /// <param name="randomAmplitude"></param>
        /// <param name="randomFrequency"></param>
        /// <param name="pauseDuration"></param>
        /// <returns></returns>
        protected virtual Vector3 DetermineNewValue(WiggleProperties properties, Vector3 newValue, Vector3 initialValue, ref Vector3 startValue, 
            ref Vector3 randomAmplitude, ref float randomFrequency, ref float pauseDuration)
        {
            switch (properties.WiggleType)
            {
                case WiggleTypes.PingPong:

                    if (properties.RelativeAmplitude)
                    {
                        if (newValue == properties.AmplitudeMin + initialValue)
                        {
                            newValue = properties.AmplitudeMax;
                            startValue = properties.AmplitudeMin;
                        }
                        else
                        {
                            newValue = properties.AmplitudeMin;
                            startValue = properties.AmplitudeMax;
                        }
                        startValue += initialValue;
                        newValue += initialValue;
                    }
                    else
                    {
                        newValue = (newValue == properties.AmplitudeMin) ? properties.AmplitudeMax : properties.AmplitudeMin;
                        startValue = (newValue == properties.AmplitudeMin) ? properties.AmplitudeMax : properties.AmplitudeMin;
                    }                    
                    RandomizeFloat(ref randomFrequency, properties.FrequencyMin, properties.FrequencyMax);
                    RandomizeFloat(ref pauseDuration, properties.PauseMin, properties.PauseMax);
                    return newValue;

                case WiggleTypes.Random:
                    startValue = newValue;
                    RandomizeFloat(ref randomFrequency, properties.FrequencyMin, properties.FrequencyMax);
                    RandomizeVector3(ref randomAmplitude, properties.AmplitudeMin, properties.AmplitudeMax);
                    RandomizeFloat(ref pauseDuration, properties.PauseMin, properties.PauseMax);
                    newValue = randomAmplitude;
                    if (properties.RelativeAmplitude)
                    {
                        newValue += initialValue;
                    }
                    return newValue;
            }
            return Vector3.zero;            
        }
        
        /// <summary>
        /// Randomizes a float between bounds
        /// </summary>
        /// <param name="randomizedFloat"></param>
        /// <param name="floatMin"></param>
        /// <param name="floatMax"></param>
        /// <returns></returns>
        protected virtual float RandomizeFloat(ref float randomizedFloat, float floatMin, float floatMax)
        {
            randomizedFloat = UnityEngine.Random.Range(floatMin, floatMax);
            return randomizedFloat;
        }

        /// <summary>
        /// Randomizes a vector3 within bounds
        /// </summary>
        /// <param name="randomizedVector"></param>
        /// <param name="vectorMin"></param>
        /// <param name="vectorMax"></param>
        /// <returns></returns>
        protected virtual Vector3 RandomizeVector3(ref Vector3 randomizedVector, Vector3 vectorMin, Vector3 vectorMax)
        {
            randomizedVector.x = UnityEngine.Random.Range(vectorMin.x, vectorMax.x);
            randomizedVector.y = UnityEngine.Random.Range(vectorMin.y, vectorMax.y);
            randomizedVector.z = UnityEngine.Random.Range(vectorMin.z, vectorMax.z);
            return randomizedVector;
        }
    }
}