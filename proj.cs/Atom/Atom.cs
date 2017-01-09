using UnityEngine;
using UnityEditor;
using AtomPackageManager.Services;
using System.Collections.Generic;

namespace AtomPackageManager
{
    public class Atom : ScriptableObject
    {
        private static Atom m_Instance;
        private static string m_ScriptImportLocation;
        private PackageManager m_PackageManager;
        private GitSourceControlService m_GitSourceControlService;
        private CodeDomCompilerService m_CodeDomCompilerService;
        private SerilizationService m_SerializationService;

        private List<Object> m_Listeners;

        public static Atom instance
        {
            get
            {
                LoadInstance();
                return m_Instance;
            }
        }

        public static string scriptImportLocation
        {
            get
            {
                if(string.IsNullOrEmpty(m_ScriptImportLocation))
                {
                    m_ScriptImportLocation = Application.dataPath.Replace("/Assets", Constants.SCRIPT_IMPORT_DIRECTORY);
                }
                return m_ScriptImportLocation;
            }
        }
        private static void LoadInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<Atom>();

                if (m_Instance == null)
                {
                    m_Instance = CreateInstance<Atom>();
                }
            }
        }


        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            LoadInstance();
        }

        public void OnEnable()
        {
            m_Listeners = new List<Object>();
            m_PackageManager = PackageManager.Load();
            m_GitSourceControlService = new GitSourceControlService();
            m_CodeDomCompilerService = new CodeDomCompilerService();
            m_SerializationService = new SerilizationService();

            AddListener(m_GitSourceControlService);
            AddListener(m_PackageManager);
            AddListener(m_CodeDomCompilerService);
            AddListener(m_SerializationService);
        }

        private void OnDisable()
        {
            RemoveListener(m_GitSourceControlService);
            RemoveListener(m_PackageManager);
            RemoveListener(m_CodeDomCompilerService);
            RemoveListener(m_SerializationService);
        }

        /// <summary>
        /// Notifies all listeners that an event has happened. 
        /// </summary>
        public static void Notify(int eventCode, object context)
        {
            var listeners = instance.m_Listeners;
            for (int i = 0; i < listeners.Count; i++)
            {
                IEventListener asEventListener = listeners[i] as IEventListener;

                asEventListener.OnNotify(eventCode, context);
            }
        }

        /// <summary>
        /// Adds an event listener to Atom's internal events. 
        /// </summary>
        public static void AddListener<T>(T listener) where T : Object, IEventListener
        {
            instance.m_Listeners.Add(listener);
        }

        /// <summary>
        /// Removes an event Listener from Atom's internal events. 
        /// </summary>
        public static void RemoveListener<T>(T listener) where T : Object, IEventListener
        {
            instance.m_Listeners.Remove(listener);
        }
    }
}
