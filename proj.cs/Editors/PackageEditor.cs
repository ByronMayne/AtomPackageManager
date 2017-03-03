using UnityEditor;
using UnityEngine;
using AtomPackageManager.Packages;
using System;
using UnityEditor.AnimatedValues;

namespace AtomPackageManager
{
    public class PackageEditor : EditorWindow
    {
        private string m_PackageSearchFilter = "";

        // Styles
        [NonSerialized]
        private bool m_StylesLoaded = false;
        private GUIStyle m_ToolbarSerachFieldStyle;
        private GUIStyle m_ToobarCancelButtonStyle;
        private GUIStyle m_AddRemovePackageStyle;



        // Split Window Settings. 
        private const float DRAG_RECT_WIDTH = 5f;
        private const float MIN_SPLIT_SIZE = 50f;
        private float m_PackageListWidth = 100;
        private Rect m_WorkingContentRect;

        private Vector2 m_PackageInfoScrollPosition;
        private int m_AssemblySelectionIndex = 0;
        private bool m_IsInitialized;

        // animated values
        private AnimBool m_AddRepositoryPackageEditorOpen;
        private AnimBool m_SettingsEditorOpen;

        // Serialized Data
        private SerializedObject m_SerializedAtom;
        private SerializedProperty m_PackageManager;
        private SerializedProperty m_PackagesList;
        private SerializedProperty m_Settings;

        private GUICarousel m_PackageCarousel;


        [SerializeField]
        private Atom m_Atom;

        public void RefreshValues()
        {
            m_SerializedAtom.Update();
            Repaint();
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
            // Set our min window this
            minSize = Vector2.one * Constants.MIN_WINDOW_SIZE;
            // Load Atom
            m_Atom = FindObjectOfType<Atom>();
            // Only init if we found Atom
            if (m_Atom != null)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            m_IsInitialized = true;
            // Grab the manager
            m_SerializedAtom = new SerializedObject(m_Atom);
            // Get our package manager
            m_PackageManager = m_SerializedAtom.FindProperty("m_PackageManager");
            // Find the nested packages.
            m_PackagesList = m_PackageManager.FindPropertyRelative("m_Packages");
            // Get our settings
            m_Settings = m_PackageManager.FindPropertyRelative("m_Settings");
            // Create Serialized Values
            m_AddRepositoryPackageEditorOpen = new AnimBool(false);
            m_SettingsEditorOpen = new AnimBool(false);
            m_AddRepositoryPackageEditorOpen.valueChanged.AddListener(Repaint);
            m_SettingsEditorOpen.valueChanged.AddListener(Repaint);

            // Create our carousel
            m_PackageCarousel = new GUICarousel(m_PackagesList, Repaint);
            m_PackageCarousel.onDrawElementCallback += OnDrawPackageElement;
        }

        private void OnDisable()
        {
            m_AddRepositoryPackageEditorOpen.valueChanged.RemoveListener(Repaint);
            m_SettingsEditorOpen.valueChanged.RemoveListener(Repaint);
        }

        private void OnGUI()
        {
            if (m_IsInitialized)
            {
                LoadStyles();
                DrawToolbar();
                m_PackageCarousel.DoGUILayout();

                if (m_PackageCarousel.selectedElement != null)
                {
                    DrawPackage(m_PackageCarousel.selectedElement);
                }
            }
            else
            {

            }
        }

        private void OnDrawPackageElement(Rect rect, SerializedProperty element, bool isSelected)
        {
            if (isSelected)
            {
                GUI.color = Color.gray;
            }

            SerializedProperty packageName = element.FindPropertyRelative("m_PackageName");
            GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
            style.fontSize = 20;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Box(rect, GUIContent.none);
            GUI.Label(rect, packageName.stringValue, style);
            GUI.color = Color.white;
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
                        atomMenu.AddItem(Labels.createNewPackageButton, false, OnCreateNewPackagePressed);
                        if (m_PackageCarousel.hasItemSelected)
                        {
                            atomMenu.AddItem(Labels.packageEditorRemoveButton, false, OnRemovePackageButtonPressed);
                        }
                        else
                        {
                            atomMenu.AddDisabledItem(Labels.packageEditorRemoveButton);
                        }
                        atomMenu.AddSeparator(string.Empty);
                        atomMenu.AddItem(Labels.packageEditorSettingsButton, m_SettingsEditorOpen.target, ToggleSettingsView);
                        atomMenu.AddDisabledItem(Labels.closeAtomButton);
                        atomMenu.DropDown(buttonRect);
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
            string projectLocation = EditorUtility.OpenFolderPanel("Project Location", FilePaths.libraryPath, "Project Location");
            AtomPackage package = AtomPackage.CreatePackage("", projectLocation);
            m_Atom.packageManager.AddPackage(package);
            m_SerializedAtom.Update();
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
        }

        private void DrawPackage(SerializedProperty selectedPackage)
        {
            m_PackageInfoScrollPosition = EditorGUILayout.BeginScrollView(m_PackageInfoScrollPosition);
            SerializedProperty assemblies = selectedPackage.FindPropertyRelative("m_Assemblies");

            // Header

            GUILayout.BeginVertical();
            {
                EditorGUILayout.PropertyField(selectedPackage.FindPropertyRelative("m_PackageName"));
                EditorGUILayout.PropertyField(selectedPackage.FindPropertyRelative("m_Version"));
                EditorGUILayout.PropertyField(selectedPackage.FindPropertyRelative("m_RepositoryURL"));
            }
            GUILayout.EndVertical();
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            {
                if (m_AssemblySelectionIndex >= assemblies.arraySize)
                {
                    m_AssemblySelectionIndex = 0;
                }

                for (int i = 0; i < assemblies.arraySize; i++)
                {
                    SerializedProperty assembly = assemblies.GetArrayElementAtIndex(i);
                    string label = assembly.displayName;

                    bool wasSelected = m_AssemblySelectionIndex == i;
                    bool active = GUILayout.Toggle(wasSelected, label, EditorStyles.miniButtonMid);

                    if (wasSelected != active && !wasSelected)
                    {
                        m_AssemblySelectionIndex = i;
                        GUIUtility.hotControl = -1;
                        GUIUtility.keyboardControl = -1;
                    }
                }
            }
            GUILayout.EndHorizontal();

            // Draw the assembly
            if (assemblies.arraySize > 0 && m_AssemblySelectionIndex < assemblies.arraySize)
            {
                SerializedProperty currentAssembly = assemblies.GetArrayElementAtIndex(m_AssemblySelectionIndex);
                SerializedProperty assemblyName = currentAssembly.FindPropertyRelative("m_AssemblyName");
                SerializedProperty addToSolution = currentAssembly.FindPropertyRelative("m_AddToProjectSolution");
                SerializedProperty compiledScripts = currentAssembly.FindPropertyRelative("m_CompiledScripts");
                SerializedProperty references = currentAssembly.FindPropertyRelative("m_References");
                SerializedProperty supportedPlatforms = currentAssembly.FindPropertyRelative("m_SupportPlatforms");

                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.fontSize = 20;

                GUILayout.Label(assemblyName.stringValue, style);
                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(4));

                GUILayout.Label("About", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(assemblyName);
                EditorGUILayout.PropertyField(addToSolution);
                GUILayout.Label("Supported Platforms", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(supportedPlatforms, true);

                GUILayout.Label("References", EditorStyles.boldLabel);
                for (int i = 0; i < references.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(references.GetArrayElementAtIndex(i), GUIContent.none);
                }

                GUILayout.Label("Compiled Scripts", EditorStyles.boldLabel);
                for (int i = 0; i < compiledScripts.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(compiledScripts.GetArrayElementAtIndex(i), GUIContent.none);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnSavePackageButtonPressed()
        {
            // Get the Package
            AtomPackage package = m_Atom.packageManager.packages[m_PackageCarousel.selectionIndex];

            // Save to disk
            SaveSerilaizedValues();

            m_Atom.packageManager.Save();
            // Apply Import Settings
            m_Atom.ApplyAtomImporterSettings(package.assemblies[0]);

        }

        private void OnCompilePackageButtonPressed()
        {
            GUIUtility.hotControl = -1;
            GUIUtility.keyboardControl = -1;

            // Get the current
            m_SerializedAtom.ApplyModifiedProperties();

            // Get the Package
            AtomPackage package = m_Atom.packageManager.packages[m_PackageCarousel.selectionIndex];

            m_Atom.CompilePackage(package);

            // Refresh our objects
            m_SerializedAtom.UpdateIfDirtyOrScript();
        }

        private void OnRemovePackageButtonPressed()
        {
            m_Atom.packageManager.packages.RemoveAt(m_PackageCarousel.selectionIndex);
            m_SerializedAtom.Update();
            GUIUtility.ExitGUI();
        }
    }
}
