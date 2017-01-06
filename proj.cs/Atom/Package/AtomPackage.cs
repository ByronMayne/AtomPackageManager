using System;
using System.Collections.Generic;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [Serializable]
    public class AtomPackage : ScriptableObject
    {
        [SerializeField]
        private string m_Version;

        [SerializeField]
        private List<AtomAssembly> m_Assemblies;

        /// <summary>
        /// The version of this Atom package.
        /// </summary>
        public string version
        {
            get { return m_Version; }
            set { m_Version = value; }
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