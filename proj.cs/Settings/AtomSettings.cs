 
using UnityEngine;
using UnityEditor;

namespace AtomPackageManager
{
    [System.Serializable]
    public class AtomSettings
    {
		[SerializeField]
        private string m_UnityPath = "/Assets/Atom";

        /// <summary>
        /// The path that Atom should put all the dlls it compiles in your Unity project.
        /// This value is saved to the project settings of Atom.
        /// </summary>
        public string UnityPath
        {
            get { return m_UnityPath; }
            set { UnityPath = value; }
        }

        /// <summary>
        /// Loads all the settings from disk.
        /// </summary>
        public void Load()
        {
        }

        /// <summary>
        /// Writes all the settings to disk.
        /// </summary>
        public void Save()
        {
		}
    }
}

