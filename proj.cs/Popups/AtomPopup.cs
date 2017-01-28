using System;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Popups
{
    public class AtomPopup : EditorWindow
    {
        private string m_Title = "Atom Popup";
        private EditorWindow m_Owner; 

        protected virtual void Initialize(EditorWindow owner)
        {
            m_Owner = owner;
            // Set it's title
            titleContent = new GUIContent(windowTitle);
            // Show it
            ShowAuxWindow();
            // Create a rect
            Rect displayRect = new Rect();
            // Get our size. 
            Vector2 popupSize = GetWindowSize();
            // Center the window to the middle of the editor window. 
            displayRect.x = ((owner.position.width - popupSize.x) * 0.5f) + owner.position.x;
            displayRect.y = ((owner.position.height - popupSize.y) * 0.5f) + owner.position.y;
            // Default the width and height. 
            displayRect.width = popupSize.x;
            displayRect.height = popupSize.y;
            // Set it's position
            position = displayRect;
            maxSize = popupSize;
            minSize = popupSize;
            // Exit the GUI
            GUIUtility.ExitGUI();
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
