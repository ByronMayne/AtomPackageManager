using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Popups
{
    public class MessagePopup : AtomPopup
    {
        public enum Type
        {

            /// <summary>
            /// No icon is used.
            /// </summary>
            Log,
            /// <summary>
            /// A white cloud icon
            /// </summary>
            Hint,
            /// <summary>
            /// A yellow explication point icon
            /// </summary>
            Warning,
            /// <summary>
            /// A red explication point icon.
            /// </summary>
            Error
        }

        protected GUIContent m_Message;
        private Type m_Type;
        protected GUIContent m_LogIcon;
        protected GUIContent m_OkayButtonLabel;

        public static void ShowSimpleMessage(string title, string message, Type logType = Type.Log)
        {
            ShowSimpleMessage(title, message, "Okay", logType);
        }

        public static void ShowSimpleMessage(string title, string message, string okayButtonName, Type logType = Type.Log)
        {
            // Make sure we are on the main thread
            if(!IsMainThread)
            {
                // Force us back on the main thread.
                EditorApplication.delayCall += () => ShowSimpleMessage(title, message, okayButtonName, logType);
                // Break out.
                return;
            }
            // Create a instance
            MessagePopup messagePopup = CreateInstance<MessagePopup>();
            // Save our type
            messagePopup.m_Type = logType;
            // Set it's title
            messagePopup.windowTitle = title;
            // Set our message
            messagePopup.m_Message = new GUIContent(message);
            // Set our okayButtonName
            messagePopup.m_OkayButtonLabel = new GUIContent(okayButtonName);
            // Load our icon
            messagePopup.LodIcon();
            // Show it
            messagePopup.Initialize();
        }

        protected void LodIcon()
        {
            if (m_Type == Type.Error)
            {
                m_LogIcon = new GUIContent("Error", EditorGUIUtility.FindTexture("console.erroricon"));
            }
            else if (m_Type == Type.Warning)
            {
                m_LogIcon = new GUIContent("Warning", EditorGUIUtility.FindTexture("console.warnicon"));
            }
            else if (m_Type == Type.Hint)
            {
                m_LogIcon = new GUIContent("Info", EditorGUIUtility.FindTexture("console.infoicon"));
            }
            else
            {
                m_LogIcon = GUIContent.none;
            }
        }

        protected override Vector2 GetWindowSize()
        {
            // Get our default size
            Vector2 size = base.GetWindowSize();
            // Get our message content size height in the y.
            size.y = messageStyle.CalcHeight(m_Message, size.x);
            // Add one row for your buttons
            size.y += EditorGUIUtility.singleLineHeight * 5;
            // Return it.
            return size;
        }

        protected override void DrawMessage()
        {
            GUILayout.Box(m_LogIcon, GUIStyle.none);

            // Draw our message
            GUILayout.Box(m_Message, messageStyle);

            GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(3.0f));
        }

        protected override void DrawContent()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(m_OkayButtonLabel))
                {
                    OnOkayButtonPressed();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected virtual void OnOkayButtonPressed()
        {
            Close();
        }
    }
}
