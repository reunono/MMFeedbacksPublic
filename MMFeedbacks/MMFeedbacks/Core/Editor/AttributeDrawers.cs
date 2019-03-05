using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Feedbacks
{
    // original implementation by http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    [CustomPropertyDrawer(typeof(MMFConditionAttribute))]
    public class MMFConditionAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MMFConditionAttribute conditionAttribute = (MMFConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(conditionAttribute, property);
            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!conditionAttribute.Hidden || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = previouslyEnabled;
        }

        private bool GetConditionAttributeResult(MMFConditionAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = true;
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionBoolean);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                enabled = sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogWarning("No matching boolean found for ConditionAttribute in object: " + condHAtt.ConditionBoolean);
            }

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MMFConditionAttribute conditionAttribute = (MMFConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(conditionAttribute, property);

            if (!conditionAttribute.Hidden || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }

    [CustomPropertyDrawer(typeof(MMFHiddenAttribute))]
    public class MMFHiddenAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }
    }

    [CustomPropertyDrawer(typeof(MMFInformationAttribute))]
    /// <summary>
    /// This class allows the display of a message box (warning, info, error...) next to a property (before or after)
    /// </summary>
    public class MMFInformationAttributeDrawer : PropertyDrawer
    {
        // determines the space after the help box, the space before the text box, and the width of the help box icon
        const int spaceBeforeTheTextBox = 5;
        const int spaceAfterTheTextBox = 10;
        const int iconWidth = 55;

        MMFInformationAttribute informationAttribute { get { return ((MMFInformationAttribute)attribute); } }

        /// <summary>
        /// OnGUI, displays the property and the textbox in the specified order
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="prop">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            if (HelpEnabled())
            {
                EditorStyles.helpBox.richText = true;
                Rect helpPosition = rect;
                Rect textFieldPosition = rect;

                if (!informationAttribute.MessageAfterProperty)
                {
                    // we position the message before the property
                    helpPosition.height = DetermineTextboxHeight(informationAttribute.Message);

                    textFieldPosition.y += helpPosition.height + spaceBeforeTheTextBox;
                    textFieldPosition.height = GetPropertyHeight(prop, label);
                }
                else
                {
                    // we position the property first, then the message
                    textFieldPosition.height = GetPropertyHeight(prop, label);

                    helpPosition.height = DetermineTextboxHeight(informationAttribute.Message);
                    // we add the complete property height (property + helpbox, as overridden in this very script), and substract both to get just the property
                    helpPosition.y += GetPropertyHeight(prop, label) - DetermineTextboxHeight(informationAttribute.Message) - spaceAfterTheTextBox;
                }

                EditorGUI.HelpBox(helpPosition, informationAttribute.Message, informationAttribute.Type);
                EditorGUI.PropertyField(textFieldPosition, prop, label, true);
            }
            else
            {
                Rect textFieldPosition = rect;
                textFieldPosition.height = GetPropertyHeight(prop, label);
                EditorGUI.PropertyField(textFieldPosition, prop, label, true);
            }
        }

        /// <summary>
        /// Returns the complete height of the whole block (property + help text)
        /// </summary>
        /// <returns>The block height.</returns>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (HelpEnabled())
            {
                return EditorGUI.GetPropertyHeight(property) + DetermineTextboxHeight(informationAttribute.Message) + spaceAfterTheTextBox + spaceBeforeTheTextBox;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property);
            }
        }

        /// <summary>
        /// Checks the editor prefs to see if help is enabled or not
        /// </summary>
        /// <returns><c>true</c>, if enabled was helped, <c>false</c> otherwise.</returns>
        protected virtual bool HelpEnabled()
        {
            bool helpEnabled = false;
            if (EditorPrefs.HasKey("MMShowHelpInInspectors"))
            {
                if (EditorPrefs.GetBool("MMShowHelpInInspectors"))
                {
                    helpEnabled = true;
                }
            }
            return helpEnabled;
        }

        /// <summary>
        /// Determines the height of the textbox.
        /// </summary>
        /// <returns>The textbox height.</returns>
        /// <param name="message">Message.</param>
        protected virtual float DetermineTextboxHeight(string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.richText = true;

            float newHeight = style.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth - iconWidth);
            return newHeight;
        }
    }

    [CustomPropertyDrawer(typeof(MMFReadOnlyAttribute))]
    public class MMFReadOnlyAttributeDrawer : PropertyDrawer
    {
        // Necessary since some properties tend to collapse smaller than their content
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        // Draw a disabled property field
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable fields
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // Enable fields
        }
    }


}
