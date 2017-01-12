using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager
{
    public static class FilePaths
    {
        private static string m_ProjectRoot;
        private static string m_ProjectSettingsPath;
        private static string m_LibraryPath;
        private static string m_PackageManagerPath;

        public static string projectRoot
        {
            get
            {
                if(string.IsNullOrEmpty(m_ProjectRoot))
                {
                    m_ProjectRoot = Application.dataPath.Replace("/Assets", "/");
                }

                return m_ProjectRoot;
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
                    m_LibraryPath = projectRoot + "Library/Atom/";
                }

                return m_LibraryPath;
            }
        }

        public static string packageManagerPath
        {
            get
            {
                if(string.IsNullOrEmpty(m_PackageManagerPath))
                {
                    m_PackageManagerPath = projectSettingsPath + "PackageManager.asset";
                }
                return m_PackageManagerPath;
            }
        }
    }
}
