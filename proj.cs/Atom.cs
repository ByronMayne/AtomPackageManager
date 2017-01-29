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

        private static string m_DataPath;

        // Our Default Templates
        private ICompilerService m_ICompilerServiceTemplate = new CodeDomCompilerService();
        private ISourceControlService m_ISourceControlServiceTemplate = new GitSourceControlService();
        private IPluginImporterService m_IPluginImporterServiceTemplate = new PluginImporterService();
        private ISolutionModifier m_ISolutionModifierTemplate = new SolutionModifier();

        public PackageManager packageManager
        {
            get { return m_PackageManager; }
        }

        public static string dataPath
        {
            get { return m_DataPath; }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {

            // Save our data path
            //  Try to grab our instance
            Atom instance = FindObjectOfType<Atom>();
            // Check if they are null
            if (instance == null)
            {
                instance = CreateInstance<Atom>();
            }
            m_DataPath = Application.dataPath;
        }

        public void Save()
        {
            m_PackageManager.Save();
        }

        private void OnEnable()
        {
            if (m_PackageManager == null)
            {
                m_PackageManager = new PackageManager();
                m_PackageManager.Load();
            }

            // Find any plugins that have been imported since reload
            ImportPluginRequest[] importRequests = FindObjectsOfType<ImportPluginRequest>();

            for (int i = 0; i < importRequests.Length; i++)
            {
                // Get the full Asset Path
                string assetPath = importRequests[i].importer.assetPath;
                // Try to get our package
                AtomAssembly assembly = packageManager.GetAssemblyByPath(assetPath);
                // If it's not null we want to reimport it
                if (assembly != null)
                {
                    ApplyAtomImporterSettings(assembly);
                }
                // Clean up or floating requests.
                DestroyImmediate(importRequests[i]);
            }

            // Append our solution
            BuildAtomSolution();
        }



        public void CompilePackage(AtomPackage package)
        {
            for (int i = 0; i < package.assemblies.Count; i++)
            {
                ICompilerService compilerService = m_ICompilerServiceTemplate.CreateCopy();
                compilerService.CompilePackage(package, i, m_PackageManager, OnCodeCompiled);
            }
        }

        private void OnCodeCompiled(ICompilerService compilerService, AtomAssembly assembly)
        {
            if (compilerService.wasSuccessful)
            {
                // Force import things
                AssetDatabase.ImportAsset(assembly.unityAssetPath);
                // force a refresh
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Unable to compile assemblies for package " + assembly.assemblyName + " read console for errors");

                var errors = compilerService.GetErrors();

                for (int i = 0; i < errors.Count; i++)
                {
                    var current = errors[i];
                    string errorMessage = current.ErrorText;
                    errorMessage += Environment.NewLine;
                    errorMessage += "Line: " + current.Line + " Column: " + current.Column;
                    errorMessage += Environment.NewLine;
                    errorMessage += "File: " + current.FileName;
                    Debug.LogError(errorMessage);
                }
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
            for (int i = 0; i < m_PackageManager.packages.Count; i++)
            {
                if (m_PackageManager.packages[i].repositoryURL == repositoryURL)
                {
                    EditorUtility.DisplayDialog("Repository Already Cloned", "The repository '" + repositoryURL + "' is already cloned on disk and in Atom", "Okay");
                    return;
                }
            }

            ISourceControlService sourceControlService = m_ISourceControlServiceTemplate.CreateCopy();
            sourceControlService.Clone(repositoryURL, workingDirectory, m_PackageManager.CloneComplete);
        }
    }
}
