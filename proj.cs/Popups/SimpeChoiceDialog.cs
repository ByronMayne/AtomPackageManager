using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Popups
{
    public class SimpeChoiceDialog : MessagePopup
    {
        protected GUIContent m_CancelButtonLabel;
        protected Action<bool> m_ChoiceCallback; 

        public static void ShowChoiceDialog(string title, string message, Action<bool> onChoiceMade)
        {
            ShowChoiceDialog(title, message, "Okay", "Cancel", onChoiceMade);
        }

        public static void ShowChoiceDialog(string title, string message, string okayButtonLabel, Action<bool> onChoiceMade)
        {
            ShowChoiceDialog(title, message, okayButtonLabel, "Cancel", onChoiceMade);
        }

        public static void ShowChoiceDialog(string title, string message, string okayButtonLabel, string cancelButtonLabel, Action<bool> onChoiceMade)
        {
            // Make sure we are on the main thread
            if(!IsMainThread)
            {
                // Force us back on the main thread.
                EditorApplication.delayCall += () => ShowChoiceDialog(title, message, okayButtonLabel, onChoiceMade);
                // Break out.
                return;
            }
            // Create a instance
            SimpeChoiceDialog simpleChoiceDialog = CreateInstance<SimpeChoiceDialog>();
            // Set it's title
            simpleChoiceDialog.windowTitle = title;
            // Save our callback
            simpleChoiceDialog.m_ChoiceCallback = onChoiceMade;
            // Set our message
            simpleChoiceDialog.m_Message = new GUIContent(message);
            // Set our okayButtonName
            simpleChoiceDialog.m_OkayButtonLabel = new GUIContent(okayButtonLabel);
            simpleChoiceDialog.m_CancelButtonLabel = new GUIContent(cancelButtonLabel);
            // Load our icon
            simpleChoiceDialog.LodIcon();
            // Show it
            simpleChoiceDialog.Initialize();
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
                if (GUILayout.Button(m_CancelButtonLabel))
                {
                    OnCancelButtonPressed();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected virtual void OnOkayButtonPressed()
        {
            if( m_ChoiceCallback != null)
            {
                m_ChoiceCallback(true);
            }
            Close();
        }

        protected virtual void OnCancelButtonPressed()
        {
            if (m_ChoiceCallback != null)
            {
                m_ChoiceCallback(false);
            }
            Close();
        }
    }
}
