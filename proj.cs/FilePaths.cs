using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager
{
    [InitializeOnLoad]
    public static class FilePaths
    {
        private static string m_DataPath;
        private static string m_ProjectRoot;
        private static string m_ProjectSettingsPath;
        private static string m_LibraryPath;
        private static string m_PackageManagerPath;
        private static string m_UnitySolutionPath;
        private static string m_ProjectName; 
        private static string m_AtomWorkingDirectory;
        private static string m_GeneratedProjectsDirectory;
        
        static FilePaths()
        {
            m_DataPath = Application.dataPath;
            m_ProjectName = Application.dataPath.Replace("/Assets", string.Empty);
            int index = m_ProjectName.LastIndexOf('/') + 1; // We don't want the slash; 
            m_ProjectName = m_ProjectName.Substring(index);

        }
        
        public static string dataPath
        {
            get { return m_DataPath; }
        }

        public static string projectRoot
        {
            get
            {
                if (string.IsNullOrEmpty(m_ProjectRoot))
                {
                    m_ProjectRoot = m_DataPath.Replace("/Assets", "/");
                }

                return m_ProjectRoot;
            }
        }

        public static string unitySolutionPath
        {
            get
            {
                if(string.IsNullOrEmpty(m_UnitySolutionPath))
                {
                    m_UnitySolutionPath = projectRoot + m_ProjectName + ".sln";
                }
                return m_UnitySolutionPath;
            }
        }

        public static string projectSettingsPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ProjectSettingsPath))
                {
                    m_ProjectSettingsPath = projectRoot + "ProjectSettings/";
                }

                return m_ProjectSettingsPath;
            }
        }

        public static string libraryPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_LibraryPath))
                {
                    m_LibraryPath = projectRoot + "Library/";
                }

                return m_LibraryPath;
            }
        }

        public static string atomWorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(m_AtomWorkingDirectory))
                {
                    m_AtomWorkingDirectory = libraryPath + "Atom/";
                    if (!Directory.Exists(m_AtomWorkingDirectory))
                    {
                        Directory.CreateDirectory(m_AtomWorkingDirectory);
                    }
                }

                return m_AtomWorkingDirectory;
            }
        }

        public static string generatedProjectsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(m_GeneratedProjectsDirectory))
                {
                    m_GeneratedProjectsDirectory = atomWorkingDirectory + "Generated Projects/";
                    if (!Directory.Exists(m_GeneratedProjectsDirectory))
                    {
                        Directory.CreateDirectory(m_GeneratedProjectsDirectory);
                    }
                }

                return m_GeneratedProjectsDirectory;
            }
        }

        public static string packageManagerPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackageManagerPath))
                {
                    m_PackageManagerPath = projectSettingsPath + "PackageManager.asset";
                }
                return m_PackageManagerPath;
            }
        }
    }
}
