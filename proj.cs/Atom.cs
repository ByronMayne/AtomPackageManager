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

        // Our Default Templates
        private ICompilerService       m_ICompilerServiceTemplate        = new CodeDomCompilerService();
        private ISourceControlService  m_ISourceControlServiceTemplate   = new GitSourceControlService();
        private IPluginImporterService m_IPluginImporterServiceTemplate  = new PluginImporterService();
        private ISolutionModifier      m_ISolutionModifierTemplate       = new SolutionModifier();

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
                m_PackageManager = new PackageManager();
                m_PackageManager.Load();
            }

            // Find any plugins that have been imported since reload
            ImportPluginRequest[] importRequests = FindObjectsOfType<ImportPluginRequest>();

            for(int i = 0; i < importRequests.Length; i++)
            {
                // Get the full Asset Path
                string assetPath = importRequests[i].importer.assetPath;
                // Try to get our package
                AtomAssembly assembly = packageManager.GetAssemblyByPath(assetPath);
                // If it's not null we want to reimport it
                if(assembly != null)
                {
                    ApplyAtomImporterSettings(assembly);
                }
                // Clean up or floating requests.
                DestroyImmediate(importRequests[i]);
            }

            // Assign our Editor Window reference if it's open
            PackageEditor editor = FindObjectOfType<PackageEditor>();
            // Check if it's null
            if(editor != null)
            {
                editor.AssignAtom(this);
            }

            // Append our solution
            BuildAtomSolution();
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

        public void BuildAtomSolution()
        {
            ISolutionModifier solutionModifier = m_ISolutionModifierTemplate.CreateCopy();
            solutionModifier.ModifySolution(Constants.SOLUTION_PATH, packageManager);
            
        }
 
        public void ApplyAtomImporterSettings(AtomAssembly assembly)
        {
            IPluginImporterService pluginImportSettings = m_IPluginImporterServiceTemplate.CreateCopy();
            pluginImportSettings.ApplyAtomImporterSettings(assembly);
        }

        public void Clone(string repositoryURL, string workingDirectory)
        {
            ISourceControlService sourceControlService = m_ISourceControlServiceTemplate.CreateCopy();
            sourceControlService.Clone(repositoryURL, workingDirectory, m_PackageManager.CloneComplete);
        }
    }
}
