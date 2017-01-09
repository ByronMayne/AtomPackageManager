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
        private List<string> m_CompiledScripts;

        [SerializeField]
        private List<string> m_References;

        [SerializeField]
        private bool m_EditorCompatible;

        [SerializeField]
        private bool m_CompatibleWithAnyPlatform;

        [SerializeField]
        private BuildTarget m_BuildTargets;

        /// <summary>
        /// The name of the assembly that gets created. 
        /// </summary>
        public string assemblyName
        {
            get { return m_AssemblyName; }
            set { m_AssemblyName = value; }
        }
        
        /// <summary>
        /// A list of paths to the scripts that will get put
        /// in this assembly. The Path is relative from the location
        /// of the atom.yaml file. 
        /// </summary>
        public List<string> compiledScripts
        {
            get { return m_CompiledScripts; }
            set { m_CompiledScripts = value; }
        }

        /// <summary>
        /// A list of references to other atom packages.
        /// </summary>
        public List<string> references
        {
            get { return m_References; }
            set { m_References = value; }
        }

        /// <summary>
        /// Will this module be imported into Unity and
        /// be enabled in the Editor?
        /// </summary>
        public bool EditorCompatible
        {
            get { return m_EditorCompatible; }
            set { m_EditorCompatible = value; }
        }

        /// <summary>
        /// Will this module be imported into Unity and run on
        /// any platform?
        /// </summary>
        public bool CompatibleWithAnyPlatform
        {
            get { return m_CompatibleWithAnyPlatform; }
            set { m_CompatibleWithAnyPlatform = value; }
        }

        /// <summary>
        /// What platforms this assembly will be enabled in. 
        /// </summary>
        public BuildTarget buildTargets
        {
            get { return m_BuildTargets; }
            set { m_BuildTargets = value; }
        }
    }
}