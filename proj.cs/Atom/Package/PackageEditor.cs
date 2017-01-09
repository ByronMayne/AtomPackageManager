using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AtomPackageManager.Packages;
using AtomPackageManager.Services;
using System;
using System.IO;

namespace AtomPackageManager
{
    public class PackageEditor : EditorWindow, IEventListener
    {

        private string m_PackageSearchFilter = "";

        // Styles
        private bool m_StylesLoaded = false;
        private GUIStyle m_ToolbarSerachFieldStyle;
        private GUIStyle m_ToobarCancelButtonStyle;
        private GUIStyle m_AddRemovePackageStyle;

        // Split Window Settings. 
        private const float DRAG_RECT_WIDTH = 5f;
        private const float MIN_SPLIT_SIZE = 50f;
        private float m_PackageListWidth = 100;
        private bool m_IsResizingList = false;
        private Rect m_WorkingContentRect;

        private Vector2 m_PackageInfoScrollPosition;
        private Vector2 m_PackageListScrollPosition;
        private int m_SelectedPackage = -1;


        // Packages
        private bool m_IsAddingPackage = false;
        private string m_NewPackageURL = "https://github.com/ByronMayne/UnityIO.git";

        // Serialized Data
        private SerializedObject m_PackageManager;
        [SerializeField]
        private List<SerializedObject> m_Packages = new List<SerializedObject>();
        private GUIContent[] m_PackageLabels = new GUIContent[0];

        private void LoadSerializedValues()
        {
            // Grab the manager
            m_PackageManager = new SerializedObject(PackageManager.Load());
            // Create our list
            m_Packages = new List<SerializedObject>();
            // Find the nested packages.
            SerializedProperty packagesList = m_PackageManager.FindProperty("m_Packages");
            // Create labels;
            m_PackageLabels = new GUIContent[packagesList.arraySize];

            // Load all valid ones
            for (int i = 0; i < packagesList.arraySize; i++)
            {
                // Get the current
                SerializedProperty current = packagesList.GetArrayElementAtIndex(i);
                // Confirm it's not null
                if (current.objectReferenceValue != null)
                {
                    // Create a new serialized object
                    SerializedObject package = new SerializedObject(current.objectReferenceValue);
                    // Add it to our list
                    m_Packages.Add(package);
                    // Add label
                    m_PackageLabels[i] = new GUIContent(package.targetObject.name);
                }
            }
        }

        private void SaveSerilaizedValues()
        {
            // Make sure no children are null
            for (int i = m_Packages.Count - 1; i >= 0; i--)
            {
                if (m_Packages[i] == null || m_Packages[i].targetObject == null)
                {
                    m_Packages.RemoveAt(i);
                }
                else if (m_Packages[i].targetObject != null)
                {
                    m_Packages[i].ApplyModifiedProperties();
                }
            }

            if (m_PackageManager != null)
            {
                // Save the parent
                m_PackageManager.ApplyModifiedProperties();
            }
        }

        private void LoadStyles()
        {
            if (m_StylesLoaded)
            {
                return;
            }

            m_ToolbarSerachFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            m_ToolbarSerachFieldStyle = new GUIStyle(m_ToolbarSerachFieldStyle);
            m_ToobarCancelButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
            m_ToobarCancelButtonStyle = new GUIStyle(m_ToobarCancelButtonStyle);
            m_AddRemovePackageStyle = new GUIStyle(EditorStyles.miniButtonMid);
            m_StylesLoaded = true;
        }

        private void OnEnable()
        {
            Atom.AddListener(this);
            LoadSerializedValues();
        }

        private void OnDisable()
        {
            Atom.RemoveListener(this);
        }

        private void OnGUI()
        {
            LoadStyles();
            DrawToolbar();
            DrawContentArea();
        }

        private void DrawToolbar()
        {
            Rect layout = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                m_PackageSearchFilter = EditorGUILayout.TextField(m_PackageSearchFilter, m_ToolbarSerachFieldStyle, GUILayout.Width(300));
                if (GUILayout.Button("", m_ToobarCancelButtonStyle))
                {
                    m_PackageSearchFilter = "";
                    GUI.FocusControl(null);
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndHorizontal();

        }

        private void DrawContentArea()
        {
            m_WorkingContentRect = new Rect(0, EditorGUIUtility.singleLineHeight, position.width, 0.0f);
            m_WorkingContentRect.height = position.height - m_WorkingContentRect.y;
            Rect packageListRect = m_WorkingContentRect;
            packageListRect.width = m_PackageListWidth;
            Rect packageInfoRect = m_WorkingContentRect;
            packageInfoRect.x += m_PackageListWidth;
            packageInfoRect.width -= m_PackageListWidth;

            DrawPackageList(packageListRect);
            DrawPackageInfo(packageInfoRect);
            HandleDrag(packageListRect);
        }

        private void HandleDrag(Rect packageListRect)
        {
            Rect dragRect = packageListRect;
            dragRect.x = m_PackageListWidth - DRAG_RECT_WIDTH;
            dragRect.width = DRAG_RECT_WIDTH * 2f;

            // Changes the mouse icon to an left right arrow to show the user they can slide the values. 
            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);

            GUI.Box(dragRect, GUIContent.none);

            if (Event.current.type == EventType.MouseDrag && m_IsResizingList)
            {
                // Update our position.
                m_PackageListWidth = Event.current.mousePosition.x;

                // Clamp our window.
                if (m_PackageListWidth < MIN_SPLIT_SIZE)
                {
                    m_PackageListWidth = MIN_SPLIT_SIZE;
                }
                else if (m_PackageListWidth > position.width - MIN_SPLIT_SIZE)
                {
                    m_PackageListWidth = position.width - MIN_SPLIT_SIZE;
                }

                // Repaint so it does not lag
                Repaint();
                // Use the drag event. 
                Event.current.Use();
            }

            // We clicked on the label rect. 
            if (Event.current.type == EventType.MouseDown && dragRect.Contains(Event.current.mousePosition))
            {
                // Use the current event. 
                Event.current.Use();
                // We are now resizing.
                m_IsResizingList = true;
            }

            // We have stopped dragging and dropping. 
            if (Event.current.type == EventType.MouseUp)
            {
                // We are not resizing. 
                m_IsResizingList = false;
            }
        }

        private void DrawPackageList(Rect packageListRect)
        {
            GUI.Box(packageListRect, GUIContent.none);

            GUILayout.BeginArea(packageListRect);
            {
                m_PackageListScrollPosition = EditorGUILayout.BeginScrollView(m_PackageListScrollPosition);
                {
                    m_SelectedPackage = GUILayout.SelectionGrid(m_SelectedPackage, m_PackageLabels, 1, GUILayout.Height(16 * m_PackageLabels.Length));
                }
                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(3.0f));

                if (!m_IsAddingPackage)
                {
                    GUILayout.BeginHorizontal();
                    {

                        if (GUILayout.Button(Labels.packageEditorAddButton, EditorStyles.miniButtonLeft))
                        {
                            m_IsAddingPackage = true;
                        }

                        if (GUILayout.Button(Labels.packageEditorRemoveButton, EditorStyles.miniButtonRight))
                        {

                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {

                        if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
                        {
                            m_IsAddingPackage = false;
                            string directory = Path.GetFileNameWithoutExtension(m_NewPackageURL);
                            GitCloneRequest request = new GitCloneRequest(m_NewPackageURL, Atom.scriptImportLocation + directory + '/');
                            Atom.Notify(Events.GIT_CLONE_REQUESTED, request);
                            m_NewPackageURL = string.Empty;
                            GUIUtility.hotControl = -1;
                            GUIUtility.keyboardControl = -1;
                        }

                        if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
                        {
                            m_IsAddingPackage = false;
                            m_NewPackageURL = string.Empty;
                            GUIUtility.hotControl = -1;
                            GUIUtility.keyboardControl = -1;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label("Package URL");
                    m_NewPackageURL = EditorGUILayout.TextField(m_NewPackageURL);
                }
                GUILayout.Space(3.0f);
            }
            GUILayout.EndArea();
        }

        private void DrawPackageInfo(Rect packageInfoRect)
        {
            packageInfoRect.x += 10f;
            GUILayout.BeginArea(packageInfoRect);
            {
                m_PackageInfoScrollPosition = EditorGUILayout.BeginScrollView(m_PackageInfoScrollPosition);
                {
                    if (m_SelectedPackage > -1 && m_SelectedPackage < m_Packages.Count)
                    {
                        SerializedProperty iterator = m_Packages[m_SelectedPackage].GetIterator();
                        iterator.Next(true);
                        while (iterator.NextVisible(false))
                        {
                            EditorGUILayout.PropertyField(iterator, true);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Select a package on the left to start editing it's contents", MessageType.Info);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        public void OnPackageAdded(AtomPackage package)
        {

        }

        public void OnPackageRemoved()
        {

        }

        public void OnNotify(int eventCode, object context)
        {
            if (eventCode == Events.ON_PACKAGE_ADDED)
            {
                LoadSerializedValues();
                Repaint();
            }
        }
    }
}
