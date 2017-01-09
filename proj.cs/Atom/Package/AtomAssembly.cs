using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [System.Serializable]
    public class AtomAssembly 
    {
        [SerializeField]
        [XmlElement(ElementName = "AssemblyName")]
        private string m_AssemblyName;

        [SerializeField]
        [XmlElement(ElementName = "UnityAssetPath")]
        private string m_UnityAssetPath; 

        [SerializeField]
        [XmlElement(ElementName = "CompiledScripts")]
        private List<string> m_CompiledScripts;

        [SerializeField]
        [XmlElement(ElementName = "References")]
        private List<string> m_References;

        [SerializeField]
        [XmlElement(ElementName = "EditorCompatible")]
        private bool m_EditorCompatible;

        [SerializeField]
        [XmlElement(ElementName = "CompatiableWithAnyPlatform")]
        private bool m_CompatibleWithAnyPlatform;

        [SerializeField]
        [XmlElement(ElementName = "BuildTarget")]
        private BuildTarget m_BuildTargets;

        /// <summary>
        /// The name of the assembly that gets created. 
        /// </summary>
        public string assemblyName
        {
            get { return m_AssemblyName; }
        }

        /// <summary>
        /// Gets the relative path that this assembly
        /// should be exported too when it's compiled.
        /// </summary>
        public string unityAssetPath
        {
            get { return m_UnityAssetPath; }
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
                return Application.dataPath.Replace("/Assets", '/' + m_UnityAssetPath);
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
        }

        /// <summary>
        /// A list of references to other atom packages.
        /// </summary>
        public List<string> references
        {
            get { return m_References; }
        }

        /// <summary>
        /// Will this module be imported into Unity and
        /// be enabled in the Editor?
        /// </summary>
        public bool EditorCompatible
        {
            get { return m_EditorCompatible; }
        }

        /// <summary>
        /// Will this module be imported into Unity and run on
        /// any platform?
        /// </summary>
        public bool CompatibleWithAnyPlatform
        {
            get { return m_CompatibleWithAnyPlatform; }
        }

        /// <summary>
        /// What platforms this assembly will be enabled in. 
        /// </summary>
        public BuildTarget buildTargets
        {
            get { return m_BuildTargets; }
        }
    }
}