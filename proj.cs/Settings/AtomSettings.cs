 
using UnityEngine;
using UnityEditor;

namespace AtomPackageManager
{
    [System.Serializable]
    public class AtomSettings
    {
        [Header("Project Settings")]
        [SerializeField]
        private string m_UnityPath = "/Assets/Atom";

        [Header("Local Values")]
        [SerializeField]
        private bool m_runInBackground = true;

        /// <summary>
        /// The path that Atom should put all the dlls it compiles in your Unity project.
        //  This value is saved to the project settings of Atom.
        /// </summary>
        public string UnityPath
        {
            get { return m_UnityPath; }
            set { m_UnityPath = value; }
        }

        /// <summary>
        /// Should Atom run in the background and check for updates?
        // This value is only saved on your local machine.
        /// </summary>
        public bool runInBackground
        {
            get { return m_runInBackground; }
            set { m_runInBackground = value; }
        }

        /// <summary>
        /// Loads all the settings from disk.
        /// </summary>
        public void Load()
        {
            UnityPath = EditorPrefs.GetString(key:PlayerSettings.productName + ":UnityPath:string", defaultValue:@"/Assets/Atom");
            runInBackground = EditorPrefs.GetBool(key:PlayerSettings.productName + ":runInBackground:bool", defaultValue:true);
        }

        /// <summary>
        /// Writes all the settings to disk.
        /// </summary>
        public void Save()
        {
            EditorPrefs.SetString(key:PlayerSettings.productName + ":UnityPath:string", value:@"/Assets/Atom");
            EditorPrefs.SetBool(key:PlayerSettings.productName + ":runInBackground:bool", value:true);
        }
    }
}

