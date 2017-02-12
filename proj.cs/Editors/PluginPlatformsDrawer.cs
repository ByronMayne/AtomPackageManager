using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Editors
{
    [CustomPropertyDrawer(typeof(PluginPlatforms))]
    public class PluginPlatformsDrawer : PropertyDrawer
    {
        // Make sure we don't init our styles multiple times.
        private static bool m_IsInitialized = false;

        // Platform Icons
        private const int TOTAL_PLATFORMS = 12;
        private static GUIContent m_AndroidBuildIcon;
        private static GUIContent m_iOSBuildIcon;
        private static GUIContent m_EditorBuildIcon;
        private static GUIContent m_StandaloneBuildIcon;
        private static GUIContent m_MetroBuildIcon;
        private static GUIContent m_PS4BuildIcon;
        private static GUIContent m_PSPBuildIcon;
        private static GUIContent m_SamsungTVBuildIcon;
        private static GUIContent m_WebGLBuildIcon;
        private static GUIContent m_AppleTVBuildIcon;
        private static GUIContent m_TizenBuildIcon;
        private static GUIContent m_3DSBuildIcon;
        private static GUIContent m_CPUPopupLabel;
        private static GUIContent m_OSPlatformLabel;
        private static GUIContent m_WindowsLabel;
        private static GUIContent m_LinuxLabel;
        private static GUIContent m_OXSLabel;
        private static GUIContent m_x86ArchitectureLabel;
        private static GUIContent m_x86_x64ArchitectureLabel;

        // This only works with one on the screen
        private int m_SelectedPlatform;

        private void Initialize()
        {
            if (!m_IsInitialized)
            {
                m_AndroidBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.Android.Small"));
                m_iOSBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.iPhone.Small"));
                m_EditorBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.Editor.Small"));
                m_StandaloneBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.Standalone.Small"));
                m_MetroBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.Metro.Small"));
                m_PS4BuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.PS4.Small"));
                m_PSPBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.PSP2.Small"));
                m_SamsungTVBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.SamsungTV.Small"));
                m_WebGLBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.WebGL"));
                m_AppleTVBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.tvOS.Small"));
                m_TizenBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.Tizen.Small"));
                m_3DSBuildIcon = new GUIContent(EditorGUIUtility.FindTexture("BuildSettings.N3DS.Small"));

                m_CPUPopupLabel = new GUIContent("CPU");
                m_OSPlatformLabel = new GUIContent("OS");
                m_WindowsLabel = new GUIContent("Windows");
                m_LinuxLabel = new GUIContent("Linux");
                m_OXSLabel = new GUIContent("Mac OS X");
                m_x86ArchitectureLabel = new GUIContent("x86");
                m_x86_x64ArchitectureLabel = new GUIContent("x86_x64");

                m_IsInitialized = true;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        private bool DoPlatformToggle(ref Rect position, int index, GUIContent icon)
        {
            bool wasSelected = index == m_SelectedPlatform;
            bool wasToggled = GUI.Toggle(position, wasSelected, icon, EditorStyles.toolbarButton);
            position.x += position.width;
            if (wasToggled && !wasSelected)
            {
                m_SelectedPlatform = index;
            }
            return m_SelectedPlatform == index;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();
            float buttonWidth = position.width / TOTAL_PLATFORMS;
            position.width = buttonWidth;

            if (DoPlatformToggle(ref position, 0, m_EditorBuildIcon))
            {
                GUILayout.Space(5);
                SerializedProperty cpuTarget = property.FindPropertyRelative("targetCPU");
                cpuTarget.intValue = EditorGUILayout.Popup(m_CPUPopupLabel.text, cpuTarget.intValue, PluginPlatforms.SUPPORTED_CPU);

                SerializedProperty osTarget = property.FindPropertyRelative("targetOS");
                osTarget.intValue = EditorGUILayout.Popup(m_OSPlatformLabel.text, osTarget.intValue, PluginPlatforms.SUPPORTED_OS);
            }
            if (DoPlatformToggle(ref position, 1, m_StandaloneBuildIcon))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(m_WindowsLabel, EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneWindowsCompatible"), m_x86ArchitectureLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneWindows64Compatible"), m_x86_x64ArchitectureLabel);
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(m_LinuxLabel, EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneLinuxCompatible"), m_x86ArchitectureLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneLinux64Compatible"), m_x86_x64ArchitectureLabel);
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(m_OXSLabel, EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneOSXIntelCompatible"), m_x86ArchitectureLabel);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("StandaloneOSXIntel64Compatible"), m_x86_x64ArchitectureLabel);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            DoPlatformToggle(ref position, 2, m_iOSBuildIcon);
            DoPlatformToggle(ref position, 3, m_AndroidBuildIcon);
            DoPlatformToggle(ref position, 4, m_AppleTVBuildIcon);
            DoPlatformToggle(ref position, 5, m_SamsungTVBuildIcon);
            DoPlatformToggle(ref position, 6, m_MetroBuildIcon);
            DoPlatformToggle(ref position, 7, m_PS4BuildIcon);
            DoPlatformToggle(ref position, 8, m_PSPBuildIcon);
            DoPlatformToggle(ref position, 9, m_TizenBuildIcon);
            DoPlatformToggle(ref position, 10, m_3DSBuildIcon);
            DoPlatformToggle(ref position, 11, m_WebGLBuildIcon);
        }
    }
}
