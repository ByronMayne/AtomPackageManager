using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [System.Serializable]
    public class AtomPackage
    {
        [SerializeField]
        private string m_PackageName = "New Package";

        [SerializeField]
        private string m_Version = string.Empty;

        [SerializeField]
        private string m_RepositoryURL = string.Empty;

        [SerializeField]
        private string m_SolutionPath = string.Empty;

        [SerializeField]
        private int m_LocalChanges = 0;

        [SerializeField]
        private int m_LocalDeletions = 1;

        [SerializeField]
        private int m_LocalNewFiles = 2;

        [SerializeField]
        private bool m_AddToProjectSolution;

        [System.NonSerialized]
        private ThreadRoutine m_CompilingRoutine; 

        [SerializeField]
        private List<AtomAssembly> m_Assemblies = new List<AtomAssembly>();

        public ThreadRoutine compilingRoutine
        {
            get
            {
                return m_CompilingRoutine;
            }
            set
            {
                m_CompilingRoutine = value;
            }
        }

        public string SourceDirectory
        {
            get
            {
                return FilePaths.atomWorkingDirectory + m_PackageName + "/";
            }
        }

        public static AtomPackage CreatePackage(string repositoryURL, string sourceDirectory)
        {
            // Create a new one
            AtomPackage newPackage = new AtomPackage();
            // Set it's name
            newPackage.m_PackageName = Path.GetFileNameWithoutExtension(repositoryURL);
            // Set it's location
            newPackage.m_RepositoryURL = repositoryURL;
            // Updates it's local changes
            newPackage.m_LocalChanges = 0;
            newPackage.m_LocalDeletions = 0;
            newPackage.m_LocalNewFiles = 0;
            // Set it's version
            newPackage.m_Version = "1.0.0.0";
            // Find the 'Assets' folder
            string[] assetsDirectory = Directory.GetDirectories(sourceDirectory, "Assets", SearchOption.AllDirectories);
            // If we have just one we are going to assume that is a Unity project
            if(assetsDirectory.Length == 1)
            {
                // Find all the scripts in the editor folders
                string[] csFiles = Directory.GetFiles(assetsDirectory[0], "*.cs", SearchOption.AllDirectories);
                List<string> editorFiles = new List<string>();
                List<string> runtimeFiles = new List<string>();

                string editorFolderSignture = "\\Editor\\";
                int startingIndex = sourceDirectory.Length;

                for (int i = 0; i < csFiles.Length; i++)
                {
                    if(csFiles[i].Contains(editorFolderSignture))
                    {
                        editorFiles.Add(csFiles[i].Substring(startingIndex, csFiles[i].Length - startingIndex));
                    }
                    else
                    {
                        runtimeFiles.Add(csFiles[i].Substring(startingIndex, csFiles[i].Length - startingIndex));
                    }
                }

                // Create Editor Assembly
                AtomAssembly editorAssembly = new AtomAssembly();
                editorAssembly.assemblyName = newPackage.packageName + "Editor";
                editorAssembly.compatibleWithAnyPlatform = false;
                editorAssembly.editorCompatible = true;
                editorAssembly.compiledScripts = editorFiles;
                editorAssembly.references = new List<string>()
                {
                    typeof(UnityEngine.GameObject).Assembly.FullName,
                    typeof(UnityEditor.Editor).Assembly.FullName,
                    typeof(System.Object).Assembly.FullName
                };
                editorAssembly.unityAssetPath = "Assets/" + newPackage.m_PackageName + "/Editor/";

                newPackage.assemblies.Add(editorAssembly);

                // Create Runtime Assembly
                AtomAssembly runtimeAssembly = new AtomAssembly();
                runtimeAssembly.assemblyName = newPackage.packageName;
                runtimeAssembly.compatibleWithAnyPlatform = true;
                runtimeAssembly.editorCompatible = true;
                runtimeAssembly.compiledScripts = runtimeFiles;
                runtimeAssembly.references = new List<string>()
                {
                    typeof(UnityEngine.GameObject).Assembly.FullName,
                    typeof(System.Object).Assembly.FullName
                };
                runtimeAssembly.unityAssetPath = "Assets/" + newPackage.m_PackageName + "/";

                newPackage.assemblies.Add(runtimeAssembly);
            }
            Debug.Log("Returning New Packge");
            return newPackage;
        }

        /// <summary>
        /// The name of this package.
        /// </summary>
        public string packageName
        {
            get { return m_PackageName; }
        }

        /// <summary>
        /// The version of this Atom package.
        /// </summary>
        public string version
        {
            get { return m_Version; }
        }

        /// <summary>
        /// The path to the solution that should be included to the Unity project. This
        /// path is relative from the root of the repo.
        /// </summary>
        public string solutionPath
        {
            get { return m_SolutionPath; }
        }

        /// <summary>
        /// The url where the content can be found.
        /// </summary>
        public string repositoryURL
        {
            get { return m_RepositoryURL; }
        }

        /// <summary>
        /// The list of generated assemblies that will be generated
        /// from this package.
        /// </summary>
        public List<AtomAssembly> assemblies
        {
            get { return m_Assemblies; }
        }
    }
}