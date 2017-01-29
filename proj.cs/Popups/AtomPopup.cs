using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Popups
{
    [InitializeOnLoad]
    public class AtomPopup : EditorWindow
    {
        private string m_Title = "Atom Popup";
        private EditorWindow m_Owner;
        protected static int m_MainThreadID;
        private GUIStyle m_MessageStyle;

        /// <summary>
        /// Invoked by Unity when it's created. We use this to capture the
        /// main thread ID so we know when we have to delay showing a dialog. 
        /// </summary>
        static AtomPopup()
        {
            m_MainThreadID = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Gets the style that we use for our messages.
        /// </summary>
        protected GUIStyle messageStyle
        {
            get
            {
                if(m_MessageStyle == null)
                {
                    m_MessageStyle = new GUIStyle(EditorStyles.label);
                    m_MessageStyle.wordWrap = true;
                    m_MessageStyle.richText = true;
                }
                return m_MessageStyle;
            }
        }

        /// <summary>
        /// Returns true if this is the current thread and false if it's not.
        /// </summary>
        protected static bool IsMainThread
        {
            get { return m_MainThreadID == Thread.CurrentThread.ManagedThreadId; }
        }

        protected virtual void Initialize()
        {
            m_Owner = GetWindow<PackageEditor>();
            // Set it's title
            titleContent = new GUIContent(windowTitle);

            // Create a rect
            Rect displayRect = new Rect();
            // Get our size. 
            Vector2 popupSize = GetWindowSize();
            // Center the window to the middle of the editor window. 
            displayRect.x = ((m_Owner.position.width - popupSize.x) * 0.5f) + m_Owner.position.x;
            displayRect.y = ((m_Owner.position.height - popupSize.y) * 0.5f) + m_Owner.position.y;
            // Default the width and height. 
            displayRect.width = popupSize.x;
            displayRect.height = popupSize.y;
            // Set it's position
            position = displayRect;
            maxSize = popupSize;
            minSize = popupSize;
            // Exit the GUI if we were invoked from a GUI scope.
            // Show it
            ShowAuxWindow();
            if (Event.current != null)
            {
                GUIUtility.ExitGUI();
            }
        }

        public virtual string windowTitle
        {
            get { return m_Title; }
            protected set { m_Title = value; }
        }

        protected virtual void OnOpen()
        {
            
        }

        private void OnGUI()
        {
            DrawMessage();
            DrawContent();
        }

        protected virtual void DrawContent()
        {

        }

        protected virtual void DrawMessage()
        {

        }

        protected virtual Vector2 GetWindowSize()
        {
            return new Vector2(400, 150);
        }

        protected virtual void OnClose()
        {

        }

        private void OnDestroy()
        {
            OnClose();
        }
    }
}
