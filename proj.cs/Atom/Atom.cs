using UnityEngine;
using UnityEditor;
using AtomPackageManager.Services;
using System.Collections.Generic;
using AtomPackageManager.Packages;
using System;
using System.CodeDom.Compiler;

namespace AtomPackageManager
{
    [System.Serializable]
    public class Atom : ScriptableObject
    {
        [SerializeField]
        private PackageManager m_PackageManager;

        // Our Default Tempaltes
        private ICompilerService       m_ICompilerServiceTemplate        = new CodeDomCompilerService();
        private ISourceControlService  m_ISourceControlServiceTemplate   = new GitSourceControlService();
        private IPluginImporterService m_IPluginImporterServiceTemplate  = new PluginImporterService();

        public PackageManager packageManager
        {
            get { return m_PackageManager; }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            //  Try to grab our instance
            Atom instance = FindObjectOfType<Atom>();
            // Check if they are null
            if(instance == null)
            {
                instance = CreateInstance<Atom>();
            }
        }


        private void OnEnable()
        {
            if(m_PackageManager == null)
            {
                m_PackageManager = new PackageManager(this);
                m_PackageManager.Load();
            }

            // Find any plugins that have been imported since reload
            ImportPluginRequest[] importRequests = FindObjectsOfType<ImportPluginRequest>();

            for(int i = 0; i < importRequests.Length; i++)
            {
                packageManager.ValidatePluginOwnership(importRequests[i].importer, ApplyAtomImporterSettings);
                DestroyImmediate(importRequests[i]);
            }

            // Assign our Editor Window reference if it's open
            PackageEditor editor = FindObjectOfType<PackageEditor>();
            // Check if it's null
            if(editor != null)
            {
                editor.AssignAtom(this);
            }
        }



        public void CompilePackage(AtomPackage package)
        {
            ICompilerService compilerService = m_ICompilerServiceTemplate.CreateCopy();
            compilerService.CompilePackage(package, OnCodeCompiled);
        }

        private void OnCodeCompiled(ICompilerService compilerService, AtomPackage package)
        {
            if(compilerService.wasSuccessful)
            {
                // Import into Unity by looping over all assemblies
                foreach (AtomAssembly assembly in package.assemblies)
                {
                    // Force import things
                    AssetDatabase.ImportAsset(assembly.unityAssetPath);
                }
                // force a refresh
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Unable to compile assemblies for package " + package.packageName);
            }
        }

        public void ApplyAtomImporterSettings(PluginImporter importer, AtomAssembly assembly)
        {
            IPluginImporterService pluginImportSettings = m_IPluginImporterServiceTemplate.CreateCopy();
            pluginImportSettings.ApplyAtomImporterSettings(importer, assembly);
        }

        public void Clone(string repositoryURL, string workingDirectory)
        {
            ISourceControlService sourceControlService = m_ISourceControlServiceTemplate.CreateCopy();
            sourceControlService.Clone(repositoryURL, workingDirectory, m_PackageManager.CloneComplete);
        }
    }
}
