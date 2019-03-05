using UnityEngine;
using UnityEditor;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A class used to regroup most of the styling options for the MMFeedback editors
    /// </summary>
    public static class MMFeedbackStyling
    {
        public static readonly GUIStyle SmallTickbox = new GUIStyle("ShurikenToggle");

        static readonly Color _splitterdark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
        static readonly Color _splitterlight = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        public static Color Splitter { get { return EditorGUIUtility.isProSkin ? _splitterdark : _splitterlight; } }

        static readonly Color _headerbackgrounddark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        static readonly Color _headerbackgroundlight = new Color(1f, 1f, 1f, 0.4f);
        public static Color HeaderBackground { get { return EditorGUIUtility.isProSkin ? _headerbackgrounddark : _headerbackgroundlight; } }

        static readonly Color _reorderdark = new Color(1f, 1f, 1f, 0.2f);
        static readonly Color _reorderlight = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        public static Color Reorder { get { return EditorGUIUtility.isProSkin ? _reorderdark : _reorderlight; } }

        static readonly Texture2D _paneoptionsicondark;
        static readonly Texture2D _paneoptionsiconlight;
        public static Texture2D PaneOptionsIcon { get { return EditorGUIUtility.isProSkin ? _paneoptionsicondark : _paneoptionsiconlight; } }

        static MMFeedbackStyling()
        {
            _paneoptionsicondark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            _paneoptionsiconlight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
        }

        /// <summary>
        /// Simply drow a splitter line and a title bellow
        /// </summary>
        static public void DrawSection(string title)
        {
            EditorGUILayout.Space();

            DrawSplitter();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draw a separator line
        /// </summary>
        static public void DrawSplitter()
        {
            // Helper to draw a separator line

            var rect = GUILayoutUtility.GetRect(1f, 1f);

            rect.xMin = 0f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, Splitter);
        }

        /// <summary>
        /// Draw a header similar to the one used for the post-process stack
        /// </summary>
        static public Rect DrawSimpleHeader(ref bool expanded, ref bool activeField, string title)
        {
            var e = Event.current;

            // Initialize Rects

            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);
            
            var reorderRect = backgroundRect;
            reorderRect.xMin -= 8f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, HeaderBackground);

            // Foldout
            expanded = GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title
            /*using (new EditorGUI.DisabledScope(!activeField))
            {
            }*/
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, SmallTickbox);
            
            // Handle events
            
            if (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return backgroundRect;
        }

        /// <summary>
        /// Draw a header similar to the one used for the post-process stack
        /// </summary>
        static public Rect DrawHeader(ref bool expanded, ref bool activeField, string title, System.Action<GenericMenu> fillGenericMenu)
        {
            var e = Event.current;

            // Initialize Rects

            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var offset = 4f;

            var reorderRect = backgroundRect;
            reorderRect.xMin -= 8f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            var labelRect = backgroundRect;
            labelRect.xMin += 32f + offset;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.xMin += offset;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.xMin += offset;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, HeaderBackground);

            // Foldout
            expanded = GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title
            using (new EditorGUI.DisabledScope(!activeField))
            {
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            }

            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, SmallTickbox);

            // Dropdown menu icon
            GUI.DrawTexture(menuRect, menuIcon);

            for(int i = 0; i < 3; i++)
            {
                Rect r = reorderRect;
                r.height = 1;
                r.y = reorderRect.y + reorderRect.height * (i / 3.0f);
                EditorGUI.DrawRect(r, Reorder);
            }

            // Handle events

            if (e.type == EventType.MouseDown)
            {
                if (menuRect.Contains(e.mousePosition))
                {
                    var menu = new GenericMenu();
                    fillGenericMenu(menu);
                    menu.DropDown(new Rect(new Vector2(menuRect.x, menuRect.yMax), Vector2.zero));
                    e.Use();
                }
            }
            
            if (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return backgroundRect;
        }
    }
}