 
using UnityEngine;
using UnityEditor;

namespace AtomPackageManager
{
    [System.Serializable]
	public class AtomSettings
	{
		private bool m_RunInBackground;
		private string m_UnityPath;
		private float m_TimeOut;
		private int m_ItemCount;

		/// <summary>
        /// Returns true if Atom should run in the background and false if it should not.
        /// This value is only saved on your local machine.
        /// </summary>
		public bool RunInBackground
		{
			get { return m_RunInBackground; }
			set { RunInBackground = value; }
		}
		/// <summary>
        /// The path that Atom should put all the dlls it compiles in your Unity project.
        /// This value is only saved on your local machine.
        /// </summary>
		public string UnityPath
		{
			get { return m_UnityPath; }
			set { UnityPath = value; }
		}
		/// <summary>
        /// How many seconds before requests time out
        /// This value is only saved on your local machine.
        /// </summary>
		public float TimeOut
		{
			get { return m_TimeOut; }
			set { TimeOut = value; }
		}
		/// <summary>
        /// How many items we have
        /// This value is only saved on your local machine.
        /// </summary>
		public int ItemCount
		{
			get { return m_ItemCount; }
			set { ItemCount = value; }
		}

        /// <summary>
        /// Loads all the settings from disk.
        /// </summary>
		public void Load()
		{
			RunInBackground = EditorPrefs.GetBool(key:PlayerSettings.productName + ":RunInBackground:bool", defaultValue:true);
			UnityPath = EditorPrefs.GetString(key:PlayerSettings.productName + ":UnityPath:string", defaultValue:@"/Assets");
			TimeOut = EditorPrefs.GetFloat(key:PlayerSettings.productName + ":TimeOut:float", defaultValue:10.0f);
			ItemCount = EditorPrefs.GetInt(key:PlayerSettings.productName + ":ItemCount:int", defaultValue:5);
		}

        /// <summary>
        /// Writes all the settings to disk.
        /// </summary>
		public void Save()
		{
			EditorPrefs.SetBool(key:PlayerSettings.productName + ":RunInBackground:bool", value:true);
			EditorPrefs.SetString(key:PlayerSettings.productName + ":UnityPath:string", value:@"/Assets");
			EditorPrefs.SetFloat(key:PlayerSettings.productName + ":TimeOut:float", value:10.0f);
			EditorPrefs.SetInt(key:PlayerSettings.productName + ":ItemCount:int", value:5);
		}
	}
}

