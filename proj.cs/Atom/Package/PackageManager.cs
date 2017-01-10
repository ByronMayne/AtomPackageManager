using AtomPackageManager.Packages;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace AtomPackageManager
{
    [System.Serializable]
    internal class PackageManager : ScriptableObject, IEventListener
    {
        public static string GetLocationOnDisk()
        {
            return Application.dataPath + "/" + Constants.PACKAGE_MANAGER_LOCATION;
        }

        /// <summary>
        /// Loads the Package Manager from disk or creates a new one if none
        /// exists.
        /// </summary>
        public static PackageManager Load()
        {
            // Create a holder
            PackageManager instance = null;

            // Try to find if any instance is currently alive. 
            instance = FindObjectOfType<PackageManager>();

            if (instance == null)
            {
                // Check if we have one to load from disk
                string saveLocation = GetLocationOnDisk();

                if (File.Exists(saveLocation))
                {
                    // We load the one from disk.
                    Object[] loadedObjects = InternalEditorUtility.LoadSerializedFileAndForget(saveLocation);
                    // Make sure we loaded something
                    if (loadedObjects.Length > 0)
                    {
                        // First value should be our Package Manager
                        instance = Instantiate(loadedObjects[0]) as PackageManager;
                        // We might have to loop over sub types
                        for (int i = 1; i < loadedObjects.Length; i++)
                        {
                            // Load each of our packages
                            AtomPackage package = Instantiate(loadedObjects[i - 1]) as AtomPackage;
                            // Add it to our instance
                            instance.m_Packages.Add(package);
                        }
                    }
                }

                if (instance == null)
                {
                    // Everything else failed creating a new one. 
                    instance = CreateInstance<PackageManager>();
                }
            }

            return instance;
        }

        [SerializeField]
        private List<AtomPackage> m_Packages = new List<AtomPackage>();

        public void SaveToDisk()
        {
            // Create a new save array
            List<Object> objectsToSave = new List<Object>();
            // Set the first element to the package manager
            objectsToSave.Add(this);
            // Set all our packages
            for (int i = m_Packages.Count - 1; i >= 0; i--)
            {
                if (m_Packages[i] != null)
                {
                    objectsToSave.Add(m_Packages[i]);
                }
                else
                {
                    m_Packages.RemoveAt(i);
                }
            }
            // Save to disk
            InternalEditorUtility.SaveToSerializedFileAndForget(objectsToSave.ToArray(), GetLocationOnDisk(), true);
        }

        /// <summary>
        /// Invoked when we have a ON_CLONE_COMPLETE event.
        /// </summary>
        /// <param name="directory"></param>
        private AtomPackage LoadPackageFromDirectory(string directory, string sourceURL)
        {
            // Try to find the atom.yaml in the root
            string[] files = Directory.GetFiles(directory, "*.atom");

            // Do we have any results?
            for (int i = 0; i < files.Length; i++)
            {
                SerilizationRequest request = new SerilizationRequest(files[i], sourceURL);
                Atom.Notify(Events.DESERIALIZATION_REQUEST, request);
            }

            if (files.Length <= 0)
            {
                Atom.Notify(Events.ERROR_ATOM_YAML_NOT_FOUND, directory);
            }
            return null;
        }

        private void OnDeSerializationComplete(AtomPackage package)
        {
            // Assign it's name
            package.name = package.packageName;
            // Add it to our array
            m_Packages.Add(package);
            // Send event that one was loaded. 
            Atom.Notify(Events.ON_PACKAGE_ADDED, package);
            // Save the new file to disk.
            SaveToDisk();
        }

        private void OnPluginImporterd(PluginImporter pluginImporter)
        {
            for (int i = 0; i < m_Packages.Count; i++)
            {
                // Get our current package
                AtomPackage package = m_Packages[i];
                // Loop over it's assemblies
                for (int x = 0; x < package.assemblies.Count; i++)
                {
                    // get our current assembly
                    AtomAssembly assembly = package.assemblies[x];
                    // Check if they have the same path
                    Debug.Log("Check: " + pluginImporter.assetPath + " | " + assembly.unityAssetPath);
                    if(string.CompareOrdinal(pluginImporter.assetPath, assembly.unityAssetPath) == 0)
                    {
                        // We have the correct assembly so we create an import request.
                        ImportPluginRequest importRequest = new ImportPluginRequest(pluginImporter, assembly);
                        // Send the request
                        Atom.Notify(Events.PLUGIN_SET_IMPORTER_SETTINGS_REQUEST, importRequest);
                        return;
                    }
                }
            }
        }

        void IEventListener.OnNotify(int eventCode, object context)
        {
            if (eventCode == Events.ON_CLONE_COMPLETE)
            {
                GitCloneRequest request = (GitCloneRequest)context;
                LoadPackageFromDirectory(request.workingDirectory, request.sourceURL);

            }
            else if (eventCode == Events.DESERIALIZATION_COMPLETE)
            {
                OnDeSerializationComplete(context as AtomPackage);
            }
            else if (eventCode == Events.PLUGIN_IMPORTED)
            {
                OnPluginImporterd(context as PluginImporter);
            }
        }
    }
}
