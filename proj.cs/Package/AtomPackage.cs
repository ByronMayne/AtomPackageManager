using System.Collections.Generic;
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
        private string m_ContentURL = string.Empty;

        [SerializeField]
        private string m_SolutionPath = string.Empty;

        [SerializeField]
        private int m_LocalChanges = 0;

        [SerializeField]
        private int m_LocalDeletions = 1;

        [SerializeField]
        private int m_LocalNewFiles = 2;

        [SerializeField]
        private List<AtomAssembly> m_Assemblies = new List<AtomAssembly>();

        /// <summary>
        /// The name of this package.
        /// </summary>
        public string packageName
        {
            get { return m_PackageName; }
            set { m_PackageName = value; }
        }

        /// <summary>
        /// The version of this Atom package.
        /// </summary>
        public string version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        /// <summary>
        /// The path to the solution that should be included to the Unity project. This
        /// path is relative from the root of the repo.
        /// </summary>
        public string solutionPath
        {
            get { return m_SolutionPath; }
            set { m_SolutionPath = value; }
        }

        /// <summary>
        /// The url where the content can be found.
        /// </summary>
        public string contentURL
        {
            get { return m_ContentURL; }
            set { m_ContentURL = value; }
        }

        /// <summary>
        /// The list of generated assemblies that will be generated
        /// from this package.
        /// </summary>
        public List<AtomAssembly> assemblies
        {
            get { return m_Assemblies; }
            set { m_Assemblies = value; }
        }
    }
}