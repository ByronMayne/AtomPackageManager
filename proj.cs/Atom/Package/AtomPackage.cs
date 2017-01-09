using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [Serializable]
    public class AtomPackage : ScriptableObject
    {
        [SerializeField]
        [XmlElement(ElementName="PackageName")]
        private string m_PackageName; 

        [SerializeField]
        [XmlElement(ElementName = "Version")]
        private string m_Version;

        [SerializeField]
        [XmlElement(ElementName = "ContentURL")]
        private string m_ContentURL;

        [SerializeField]
        [XmlElement(ElementName = "Assemblies")]
        private List<AtomAssembly> m_Assemblies;

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