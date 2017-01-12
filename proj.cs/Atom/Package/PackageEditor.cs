using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AtomPackageManager.Packages;
using AtomPackageManager.Services;
using System;
using System.IO;

namespace AtomPackageManager
{
    public class PackageEditor : EditorWindow
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
        private SerializedObject m_SerializedAtom;
        [SerializeField]
        private GUIContent[] m_PackageLabels = new GUIContent[0];
        private Atom m_Atom;

        public void LoadSerializedValues()
        {
            if (m_Atom != null)
            {
                // Grab the manager
                m_SerializedAtom =  new SerializedObject(m_Atom);
                // Get our package manager
                SerializedProperty packageManager = m_SerializedAtom.FindProperty("m_PackageManager");
                // Find the nested packages.
                SerializedProperty packagesList = packageManager.FindPropertyRelative ("m_Packages");
                // Create labels;
                m_PackageLabels = new GUIContent[packagesList.arraySize];
                // Create names
                for (int i = 0; i < packagesList.arraySize; i++)
                {
                    var current = packagesList.GetArrayElementAtIndex(i);
                    // Get the name
                    m_PackageLabels[i] = new GUIContent(current.FindPropertyRelative("m_PackageName").stringValue);
                }

            }
        }

        public void AssignAtom(Atom atom)
        {
            m_Atom = atom;
            LoadSerializedValues();
        }

        private void SaveSerilaizedValues()
        {
            if (m_SerializedAtom != null)
            {
                // Save the parent
                m_SerializedAtom.ApplyModifiedProperties();
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
            AssignAtom(FindObjectOfType<Atom>());
            LoadSerializedValues();
        }

        private void OnGUI()
        {
            if (m_SerializedAtom != null)
            {
                LoadStyles();
                DrawToolbar();
                DrawContentArea();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
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
                            string workingDirectory = Path.GetFileNameWithoutExtension(m_NewPackageURL);
                            m_Atom.Clone(m_NewPackageURL, FilePaths.libraryPath + workingDirectory);
                            GUIUtility.hotControl = -1;
                            GUIUtility.keyboardControl = -1;
                        }

                        if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
                        {
                            m_IsAddingPackage = false;
                            //m_NewPackageURL = string.Empty;
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
            GUILayout.BeginArea(packageInfoRect);
            {
                m_PackageInfoScrollPosition = EditorGUILayout.BeginScrollView(m_PackageInfoScrollPosition);
                {
                    SerializedProperty packages = m_SerializedAtom.FindProperty("m_PackageManager").FindPropertyRelative("m_Packages");

                    if (m_SelectedPackage > -1 && m_SelectedPackage < packages.arraySize)
                    {
                        EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(m_SelectedPackage), true);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Select a package on the left to start editing it's contents", MessageType.Info);
                    }
                }
                EditorGUILayout.EndScrollView();

                GUILayout.BeginHorizontal(EditorStyles.textArea);
                {
                    if (GUILayout.Button(Labels.packageEditorSaveButton))
                    {

                    }

                    if (GUILayout.Button(Labels.packageCompileButton))
                    {
                        OnCompilePackageButtonPressed();
                    }

                    if (GUILayout.Button(Labels.packageEditorRemoveButton))
                    {
                        OnRemovePackageButtonPressed(); 
                    }
                }

                GUILayout.EndHorizontal();

            }
            GUILayout.EndArea();
        }

        private void OnSavePackageButtonPressed()
        {
            // Save to disk
            SaveSerilaizedValues();
            // Apply Import Settings
           
        }

        private void OnCompilePackageButtonPressed()
        {
            // Get the current
            m_SerializedAtom.ApplyModifiedProperties();

            // Get the Package
            AtomPackage package = m_Atom.packageManager.packages[m_SelectedPackage];

            m_Atom.CompilePackage(package);
        }
        

        private void OnRemovePackageButtonPressed()
        {

        }
    }
}
