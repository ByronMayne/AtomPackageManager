using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AtomPackageManager.Packages;
using AtomPackageManager.Services;
using System;
using System.IO;
using UnityEditor.AnimatedValues;

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
        private int m_PackageSelectionIndex = -1;

        // Packages
        private bool m_IsAddingPackage = false;
        private string m_NewPackageURL = "https://github.com/ByronMayne/UnityIO.git";

        // animated values
        private AnimBool m_AddRepositoryPackageEditorOpen;
        private AnimBool m_SettingsEditorOpen;

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
                m_SerializedAtom = new SerializedObject(m_Atom);
                // Get our package manager
                SerializedProperty packageManager = m_SerializedAtom.FindProperty("m_PackageManager");
                // Find the nested packages.
                SerializedProperty packagesList = packageManager.FindPropertyRelative("m_Packages");
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

            // Create Serialized Values
            m_AddRepositoryPackageEditorOpen = new AnimBool(false);
            m_SettingsEditorOpen = new AnimBool(false);
            m_AddRepositoryPackageEditorOpen.valueChanged.AddListener(Repaint);
            m_SettingsEditorOpen.valueChanged.AddListener(Repaint);
        }

        private void OnDisable()
        {
            m_AddRepositoryPackageEditorOpen.valueChanged.RemoveListener(Repaint);
            m_SettingsEditorOpen.valueChanged.RemoveListener(Repaint);
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
                GUILayout.BeginHorizontal(GUILayout.Width(m_PackageListWidth - 6));
                {
                    Rect buttonRect = GUILayoutUtility.GetRect(Labels.menuButton, EditorStyles.toolbarButton);

                    if (GUI.Button(buttonRect, Labels.menuButton, EditorStyles.toolbarButton))
                    {
                        GenericMenu atomMenu = new GenericMenu(); ;
                        atomMenu.AddItem(Labels.addExistingPackageButton, false, OnAddExisitingPackageButtonPressed);
                        atomMenu.AddItem(Labels.clonePackageButton, false, OnCloneNewPackagePressed);
                        //atomMenu.AddItem(Labels.createNewPackageButton, false, OnCreateNewPackagePressed);
                        atomMenu.AddDisabledItem(Labels.createNewPackageButton);
                        atomMenu.AddSeparator(string.Empty);
                        //atomMenu.AddItem(Labels.closeAtomButton, false, OnQuitPressed);
                        atomMenu.AddItem(Labels.packageEditorSettingsButton, m_SettingsEditorOpen.target, ToggleSettingsView);
                        atomMenu.AddDisabledItem(Labels.closeAtomButton);
                        atomMenu.DropDown(buttonRect);
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("+", EditorStyles.toolbarButton))
                    {
                        OnAddExisitingPackageButtonPressed();
                    }

                    if (GUILayout.Button("-", EditorStyles.toolbarButton))
                    {
                        OnRemovePackageButtonPressed();
                    }

                }
                GUILayout.EndHorizontal();

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

        /// <summary>
        /// Invoked when the user uses the menu button to say they want to add
        /// a project that is already on disk.
        /// </summary>
        private void OnAddExisitingPackageButtonPressed()
        {
            // Get our file path. 
            string atomFilePath = EditorUtility.OpenFilePanel("Atom File", Application.dataPath, "atom");
            // Check if the path is null if the user cancels the path would be empty.
            if (!string.IsNullOrEmpty(atomFilePath))
            {
                // Try to load the file
                m_Atom.packageManager.LoadAtomFileAtPath(atomFilePath);
            }
            m_SerializedAtom.Update();
        }

        private void OnCloneNewPackagePressed()
        {
            m_AddRepositoryPackageEditorOpen.target = !m_AddRepositoryPackageEditorOpen.target;
        }

        private void OnCreateNewPackagePressed()
        {

        }

        /// <summary>
        /// Toggles the viewing state of the settings menu.
        /// </summary>
        private void ToggleSettingsView()
        {
            m_SettingsEditorOpen.target = !m_SettingsEditorOpen.target;
        }

        private void OnQuitPressed()
        {
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

            DrawLeftPanel(packageListRect);
            DrawContentArea(packageInfoRect);
            HandleDrag(packageListRect);
        }

        private void HandleDrag(Rect packageListRect)
        {
            Rect dragRect = packageListRect;
            dragRect.x = m_PackageListWidth - DRAG_RECT_WIDTH;
            dragRect.width = DRAG_RECT_WIDTH * 2f;

            // Changes the mouse icon to an left right arrow to show the user they can slide the values. 
            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);

            //GUI.Box(dragRect, GUIContent.none);

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

        private void DrawLeftPanel(Rect packageListRect)
        {
            if (m_SerializedAtom == null)
            {
                return;
            }

            GUILayout.BeginArea(packageListRect);
            {
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    if (EditorGUILayout.BeginFadeGroup(m_AddRepositoryPackageEditorOpen.faded))
                    {
                        m_NewPackageURL = EditorGUILayout.TextField(m_NewPackageURL);
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
                            {
                                m_Atom.Clone(m_NewPackageURL, Constants.SCRIPT_IMPORT_DIRECTORY);
                                m_AddRepositoryPackageEditorOpen.target = false;
                            }

                            if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
                            {
                                m_AddRepositoryPackageEditorOpen.target = false;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFadeGroup();

                m_PackageListScrollPosition = EditorGUILayout.BeginScrollView(m_PackageListScrollPosition, (GUIStyle)"AnimationCurveEditorBackground");
                {
                    SerializedProperty packageManager = m_SerializedAtom.FindProperty("m_PackageManager");
                    SerializedProperty packages = packageManager.FindPropertyRelative("m_Packages");

                    GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonMid);


                    buttonStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 2;
                    buttonStyle.alignment = TextAnchor.MiddleLeft;
                    buttonStyle.contentOffset = new Vector2(10, 0);
                    buttonStyle.margin = new RectOffset(0, 0, 2, 0);
                    buttonStyle.fontStyle = FontStyle.Bold;

                    for (int i = 0; i < packages.arraySize; i++)
                    {
                        SerializedProperty current = packages.GetArrayElementAtIndex(i);
                        if (current != null)
                        {

                            SerializedProperty name = current.FindPropertyRelative("m_PackageName");

                            GUIContent label = new GUIContent(name.stringValue);
                            Rect toggleRect = GUILayoutUtility.GetRect(label, buttonStyle);

                            bool startingValue = m_PackageSelectionIndex == i;
                            bool isMouseOver = toggleRect.Contains(Event.current.mousePosition);
                            int controlID = GUIUtility.GetControlID(FocusType.Passive, toggleRect);

                            if (Event.current.type == EventType.Repaint)
                            {
                                buttonStyle.Draw(toggleRect, label, controlID, m_PackageSelectionIndex == i);
                            }

                            switch (Event.current.GetTypeForControl(controlID))
                            {
                                case EventType.MouseDown:
                                    if (toggleRect.Contains(Event.current.mousePosition))
                                    {
                                        GUIUtility.hotControl = controlID;
                                        if (m_PackageSelectionIndex != i)
                                        {
                                            m_PackageSelectionIndex = i;
                                            GUIUtility.hotControl = 0;
                                            GUIUtility.keyboardControl = 0;
                                        }
                                        Event.current.Use();
                                    }
                                    break;
                                case EventType.MouseUp:
                                    if (GUIUtility.hotControl == controlID)
                                    {
                                        GUIUtility.hotControl = 0;
                                        Event.current.Use();
                                        GUI.changed = true;
                                    }
                                    break;
                                case EventType.MouseDrag:
                                    if (GUIUtility.hotControl == controlID)
                                    {
                                        Event.current.Use();
                                    }
                                    break;
                                case EventType.Repaint:
                                    {
                                        buttonStyle.Draw(toggleRect, label, i == m_PackageSelectionIndex && (GUI.enabled || controlID == GUIUtility.hotControl) && (controlID == GUIUtility.hotControl || GUIUtility.hotControl == 0), controlID == GUIUtility.hotControl && GUI.enabled, i == m_PackageSelectionIndex, false);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            packages.DeleteArrayElementAtIndex(i);
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndScrollView();


                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(3.0f));

                if (EditorGUILayout.BeginFadeGroup(m_SettingsEditorOpen.faded))
                {
                    GUILayout.Label(Labels.settingsLocalCatagory, EditorStyles.boldLabel);

                    GUILayout.Label(Labels.settingsProjectCatagory, EditorStyles.boldLabel);
                }
                EditorGUILayout.EndFadeGroup();
            }
            GUILayout.EndArea();
        }

        private void DrawContentArea(Rect packageInfoRect)
        {
            SerializedProperty packages = m_SerializedAtom.FindProperty("m_PackageManager").FindPropertyRelative("m_Packages");
            bool hasPackageSelected = m_PackageSelectionIndex > -1 && m_PackageSelectionIndex < packages.arraySize;
            GUILayout.BeginArea(packageInfoRect);
            {
                m_PackageInfoScrollPosition = EditorGUILayout.BeginScrollView(m_PackageInfoScrollPosition);
                {


                    if (hasPackageSelected)
                    {
                        EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(m_PackageSelectionIndex), true);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Select a package on the left to start editing it's contents", MessageType.Info);
                    }
                }
                EditorGUILayout.EndScrollView();

                if (hasPackageSelected)
                {
                    GUILayout.BeginHorizontal(EditorStyles.textArea);
                    {
                        if (GUILayout.Button(Labels.packageEditorSaveButton))
                        {
                            OnSavePackageButtonPressed();
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
                }

                GUILayout.EndHorizontal();

            }
            GUILayout.EndArea();
        }

        private void OnSavePackageButtonPressed()
        {
            // Get the Package
            AtomPackage package = m_Atom.packageManager.packages[m_PackageSelectionIndex];

            // Save to disk
            SaveSerilaizedValues();

            m_Atom.packageManager.Save();
            // Apply Import Settings
            m_Atom.ApplyAtomImporterSettings(package.assemblies[0]);

        }

        private void OnCompilePackageButtonPressed()
        {
            // Get the current
            m_SerializedAtom.ApplyModifiedProperties();

            // Get the Package
            AtomPackage package = m_Atom.packageManager.packages[m_PackageSelectionIndex];

            m_Atom.CompilePackage(package);
        }

        private void OnRemovePackageButtonPressed()
        {
            m_Atom.packageManager.packages.RemoveAt(m_PackageSelectionIndex);
            m_SerializedAtom.Update();
            GUIUtility.ExitGUI();
        }
    }
}
