using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MMWiggle))]
    public class MMWiggleEditor : Editor
    {
        public struct WiggleEditorProperties
        {
            public SerializedProperty WigglePermitted;

            public SerializedProperty WiggleType;
            public SerializedProperty UseUnscaledTime;
            public SerializedProperty RelativeAmplitude;
            public SerializedProperty StartWigglingAutomatically;

            public SerializedProperty SmoothPingPong;
            public SerializedProperty UseSpeedCurve;
            public SerializedProperty SpeedCurve;

            public SerializedProperty FrequencyMin;
            public SerializedProperty FrequencyMax;

            public SerializedProperty AmplitudeMin;
            public SerializedProperty AmplitudeMax;
            public SerializedProperty PauseMin;
            public SerializedProperty PauseMax;

            public SerializedProperty NoiseFrequencyMin;
            public SerializedProperty NoiseFrequencyMax;
            public SerializedProperty NoiseShiftMin;
            public SerializedProperty NoiseShiftMax;

            public SerializedProperty LimitedTime;
            public SerializedProperty LimitedTimeTotal;
            public SerializedProperty LimitedTimeLeft;
            public SerializedProperty LimitedTimeFalloff;
            public SerializedProperty LimitedTimeResetValue;
        }
        
        protected SerializedProperty _positionActive;
        protected SerializedProperty _rotationActive;
        protected SerializedProperty _scaleActive;

        protected SerializedProperty _positionProperties;
        protected SerializedProperty _rotationProperties;
        protected SerializedProperty _scaleProperties;

        protected WiggleEditorProperties _positionEditorProperties;
        protected WiggleEditorProperties _rotationEditorProperties;
        protected WiggleEditorProperties _scaleEditorProperties;

        protected SerializedProperty _debugWiggleDuration;

        protected MMWiggle _mmWiggle;

        public bool StartWigglingAutomatically = true;

        protected virtual void OnEnable()
        {
            _mmWiggle = (MMWiggle)target;

            _positionProperties = serializedObject.FindProperty("PositionWiggleProperties");
            _rotationProperties = serializedObject.FindProperty("RotationWiggleProperties");
            _scaleProperties = serializedObject.FindProperty("ScaleWiggleProperties");

            _positionActive = serializedObject.FindProperty("PositionActive");
            _rotationActive = serializedObject.FindProperty("RotationActive");
            _scaleActive = serializedObject.FindProperty("ScaleActive");

            _debugWiggleDuration = serializedObject.FindProperty("DebugWiggleDuration");

            InitializeProperties(_positionProperties, ref _positionEditorProperties);
            InitializeProperties(_rotationProperties, ref _rotationEditorProperties);
            InitializeProperties(_scaleProperties, ref _scaleEditorProperties);
        }

        protected virtual void InitializeProperties(SerializedProperty targetProperty, ref WiggleEditorProperties editorProperties)
        {
            editorProperties.WigglePermitted = targetProperty.FindPropertyRelative("WigglePermitted");
            editorProperties.WiggleType = targetProperty.FindPropertyRelative("WiggleType");
            editorProperties.UseUnscaledTime = targetProperty.FindPropertyRelative("UseUnscaledTime");
            editorProperties.RelativeAmplitude = targetProperty.FindPropertyRelative("RelativeAmplitude");
            editorProperties.StartWigglingAutomatically = targetProperty.FindPropertyRelative("StartWigglingAutomatically");
            editorProperties.SmoothPingPong = targetProperty.FindPropertyRelative("SmoothPingPong");
            editorProperties.UseSpeedCurve = targetProperty.FindPropertyRelative("UseSpeedCurve");
            editorProperties.SpeedCurve = targetProperty.FindPropertyRelative("SpeedCurve");
            editorProperties.FrequencyMin = targetProperty.FindPropertyRelative("FrequencyMin");
            editorProperties.FrequencyMax = targetProperty.FindPropertyRelative("FrequencyMax");
            editorProperties.AmplitudeMin = targetProperty.FindPropertyRelative("AmplitudeMin");
            editorProperties.AmplitudeMax = targetProperty.FindPropertyRelative("AmplitudeMax");
            editorProperties.PauseMin = targetProperty.FindPropertyRelative("PauseMin");
            editorProperties.PauseMax = targetProperty.FindPropertyRelative("PauseMax");
            editorProperties.NoiseFrequencyMin = targetProperty.FindPropertyRelative("NoiseFrequencyMin");
            editorProperties.NoiseFrequencyMax = targetProperty.FindPropertyRelative("NoiseFrequencyMax");
            editorProperties.NoiseShiftMin = targetProperty.FindPropertyRelative("NoiseShiftMin");
            editorProperties.NoiseShiftMax = targetProperty.FindPropertyRelative("NoiseShiftMax");
            editorProperties.LimitedTime = targetProperty.FindPropertyRelative("LimitedTime");
            editorProperties.LimitedTimeTotal = targetProperty.FindPropertyRelative("LimitedTimeTotal");
            editorProperties.LimitedTimeLeft = targetProperty.FindPropertyRelative("LimitedTimeLeft");
            editorProperties.LimitedTimeFalloff = targetProperty.FindPropertyRelative("LimitedTimeFalloff");
            editorProperties.LimitedTimeResetValue = targetProperty.FindPropertyRelative("LimitedTimeResetValue");
    }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            Undo.RecordObject(target, "Modified MMWiggle");
            

            EditorGUILayout.Space();

            MMFeedbackStyling.DrawSplitter();
            DrawValueEditor("Position", _positionActive, _positionEditorProperties, _mmWiggle.PositionWiggleProperties.WiggleType);
            DrawValueEditor("Rotation", _rotationActive, _rotationEditorProperties, _mmWiggle.RotationWiggleProperties.WiggleType);
            DrawValueEditor("Scale", _scaleActive, _scaleEditorProperties, _mmWiggle.ScaleWiggleProperties.WiggleType);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_debugWiggleDuration);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Wiggle Position", EditorStyles.miniButtonLeft))
                {
                    _mmWiggle.WigglePosition(_debugWiggleDuration.floatValue);
                }
                if (GUILayout.Button("Wiggle Rotation", EditorStyles.miniButtonMid))
                {
                    _mmWiggle.WiggleRotation(_debugWiggleDuration.floatValue);
                }
                if (GUILayout.Button("Wiggle Scale", EditorStyles.miniButtonRight))
                {
                    _mmWiggle.WiggleScale(_debugWiggleDuration.floatValue);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawValueEditor(string title, SerializedProperty propertyActive, WiggleEditorProperties editorProperties, WiggleTypes wiggleType)
        {
            bool propertyIsExpanded = propertyActive.isExpanded;
            bool propertyIsActive = propertyActive.boolValue;


            Rect headerRect = MMFeedbackStyling.DrawSimpleHeader(
                        ref propertyIsExpanded,
                        ref propertyIsActive,
                        title);

            propertyActive.isExpanded = propertyIsExpanded;
            propertyActive.boolValue = propertyIsActive;

            if (propertyIsExpanded)
            {
                EditorGUI.BeginDisabledGroup(!propertyIsActive);

                EditorGUILayout.PropertyField(editorProperties.WigglePermitted);
                EditorGUILayout.PropertyField(editorProperties.WiggleType);
                EditorGUILayout.PropertyField(editorProperties.UseUnscaledTime);

                if ((wiggleType == WiggleTypes.PingPong) || (wiggleType == WiggleTypes.Random))
                {
                    if (wiggleType == WiggleTypes.PingPong)
                    {
                        EditorGUILayout.PropertyField(editorProperties.SmoothPingPong);
                    }
                    EditorGUILayout.PropertyField(editorProperties.UseSpeedCurve);
                    if (editorProperties.UseSpeedCurve.boolValue)
                    {
                        EditorGUILayout.PropertyField(editorProperties.SpeedCurve);
                    }
                    EditorGUILayout.PropertyField(editorProperties.AmplitudeMin);
                    EditorGUILayout.PropertyField(editorProperties.AmplitudeMax);
                    EditorGUILayout.PropertyField(editorProperties.RelativeAmplitude);
                    EditorGUILayout.PropertyField(editorProperties.FrequencyMin);
                    EditorGUILayout.PropertyField(editorProperties.FrequencyMax);
                    EditorGUILayout.PropertyField(editorProperties.PauseMin);
                    EditorGUILayout.PropertyField(editorProperties.PauseMax);
                }

                if (wiggleType == WiggleTypes.Noise)
                {
                    EditorGUILayout.PropertyField(editorProperties.AmplitudeMin);
                    EditorGUILayout.PropertyField(editorProperties.AmplitudeMax);
                    EditorGUILayout.PropertyField(editorProperties.RelativeAmplitude);
                    EditorGUILayout.PropertyField(editorProperties.NoiseFrequencyMin);
                    EditorGUILayout.PropertyField(editorProperties.NoiseFrequencyMax);
                    EditorGUILayout.PropertyField(editorProperties.NoiseShiftMin);
                    EditorGUILayout.PropertyField(editorProperties.NoiseShiftMax);
                }

                EditorGUILayout.PropertyField(editorProperties.LimitedTime);
                if (editorProperties.LimitedTime.boolValue)
                {
                    EditorGUILayout.PropertyField(editorProperties.LimitedTimeTotal);
                    EditorGUILayout.PropertyField(editorProperties.LimitedTimeLeft);
                    EditorGUILayout.PropertyField(editorProperties.LimitedTimeFalloff);
                    EditorGUILayout.PropertyField(editorProperties.LimitedTimeResetValue);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }
            MMFeedbackStyling.DrawSplitter();
        }
    }
}