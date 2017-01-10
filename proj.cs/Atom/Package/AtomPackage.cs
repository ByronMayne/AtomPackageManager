﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace AtomPackageManager.Packages
{
    [Serializable]
    public class AtomPackage : ScriptableObject
    {
        [SerializeField]
        private string m_PackageName = "New Package";

        [SerializeField]
        private string m_Version = "1.0.0.0";

        [SerializeField]
        private string m_ContentURL = "Not Set";

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