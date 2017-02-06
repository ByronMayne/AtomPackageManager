using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [System.Serializable]
    public class AtomAssembly
    {
        [SerializeField]
        private string m_AssemblyName;

        [SerializeField]
        private string m_UnityAssetPath;

        [SerializeField]
        private string m_Version = "0.0.0.0";

        [SerializeField]
        private bool m_AddToProjectSolution = true;

        [SerializeField]
        private List<string> m_CompiledScripts = new List<string>();

        [SerializeField]
        [AssemblyNameAttribute]
        private List<string> m_References;

        [SerializeField]
        private PluginPlatforms m_SupportPlatforms = new PluginPlatforms();

        [System.NonSerialized]
        private bool m_IsQueuedForCompile = false;
        [System.NonSerialized]
        private ThreadRoutine m_CompilingRoutine;


        public bool isQueuedForCompile
        {
            get { return m_IsQueuedForCompile; }
            set { m_IsQueuedForCompile = value; }
        }

        public ThreadRoutine compilingRoutine
        {
            get { return m_CompilingRoutine; }
            set { m_CompilingRoutine = value; }
        }

        /// <summary>
        /// The name of the assembly that gets created. 
        /// </summary>
        public string assemblyName
        {
            get { return m_AssemblyName; }
#if DEVELOPMENT
            set { m_AssemblyName = value; }
#endif 
        }

        public string fullName
        {
            get { return m_AssemblyName + ", Version=" + m_Version + " Culture=neutral, PublicKeyToken=null"; }
        }

        /// <summary>
        /// Gets the relative path that this assembly
        /// should be exported too when it's compiled.
        /// </summary>
        public string unityAssetPath
        {
            get { return m_UnityAssetPath; }
#if DEVELOPMENT
            set { m_UnityAssetPath = value; }
#endif
        }

        /// <summary>
        /// If true a cs project will be generated and 
        /// appended to this projects solution.
        /// </summary>
        public bool addToProjectSolution
        {
            get { return m_AddToProjectSolution; }
#if DEVELOPMENT
            set { m_AddToProjectSolution = value; }
#endif
        }

        public string unityAssemblyPath
        {
            get { return m_UnityAssetPath + assemblyName + ".dll"; }
        }

        /// <summary>
        /// Gets the path from the root directory to
        /// where this assembly will be put on disk in
        /// the Unity project.
        /// </summary>
        public string systemAssetPath
        {
            get
            {
                return FilePaths.projectRoot + m_UnityAssetPath;
            }
        }

        /// <summary>
        /// A list of paths to the scripts that will get put
        /// in this assembly. The Path is relative from the location
        /// of the atom.yaml file. 
        /// </summary>
        public List<string> compiledScripts
        {
            get { return m_CompiledScripts; }
#if DEVELOPMENT
            set { m_CompiledScripts = value; }
#endif
        }

        /// <summary>
        /// A list of references to other atom packages.
        /// </summary>
        public List<string> references
        {
            get { return m_References; }
#if DEVELOPMENT
            set { m_References = value; }
#endif
        }

        /// <summary>
        /// Will this module be imported into Unity and
        /// be enabled in the Editor?
        /// </summary>
        public bool editorCompatible
        {
            get { return m_SupportPlatforms.editorCompatible; }
#if DEVELOPMENT
            set { m_SupportPlatforms.editorCompatible = value; }
#endif
        }

        /// <summary>
        /// Will this module be imported into Unity and run on
        /// any platform?
        /// </summary>
        public bool compatibleWithAnyPlatform
        {
            get { return false; ; }
#if DEVELOPMENT
            set { }
#endif
        }

        /// <summary>
        /// What platforms this assembly will be enabled in. 
        /// </summary>
        public PluginPlatforms supportedPlatforms
        {
            get { return m_SupportPlatforms; }
#if DEVELOPMENT
            set { m_SupportPlatforms = value; }
#endif
        }
    }
}