using AtomPackageManager.Packages;
using System.Collections.Generic;
using System.IO;
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

            if(instance == null)
            {
                // Check if we have one to load from disk
                string saveLocation = GetLocationOnDisk();

                if(File.Exists(saveLocation))
                {
                    // We load the one from disk.
                    Object[] loadedObjects = InternalEditorUtility.LoadSerializedFileAndForget(saveLocation); 
                    // Make sure we loaded something
                    if(loadedObjects.Length > 0)
                    {
                        // First value should be our Package Manager
                        instance = Instantiate(loadedObjects[0]) as PackageManager;
                        // We might have to loop over sub types
                        for(int i = 1; i < loadedObjects.Length; i++)
                        {
                            // Load each of our packages
                            AtomPackage package = Instantiate(loadedObjects[i - 1]) as AtomPackage;
                            // Add it to our instance
                            instance.m_Packages.Add(package);
                        }
                    }
                }

                if(instance == null)
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
                if(m_Packages[i] != null)
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
        private AtomPackage LoadPackageFromDirectory(string directory)
        {
            // Try to find the atom.yaml in the root
            string[] files = Directory.GetFiles(directory, "atom.yaml");

            // Do we have any results?
            if(files.Length > 0)
            {
                // Load the Atom file using the first result.
                Object[] loadedObjects = InternalEditorUtility.LoadSerializedFileAndForget(files[0]);
                // Loop over them all
                for(int i = 0; i < loadedObjects.Length; i++)
                {
                    // Make sure it's the correct type.
                    if(loadedObjects[i] is AtomPackage)
                    {
                        // String get the name
                        AtomPackage package = Instantiate(loadedObjects[i]) as AtomPackage;
                        // Assign it's name
                        package.name = package.packageName;
                        // Add it
                        OnPackageAdded(package);
                        // Return it
                        return package;
                    }
                }
            }
            else
            {
                Atom.Notify(Events.ERROR_ATOM_YAML_NOT_FOUND, directory);
            }
            return null;
        }

        private void OnPackageAdded(AtomPackage package)
        {
            m_Packages.Add(package);
            Atom.Notify(Events.ON_PACKAGE_ADDED, package);
            SaveToDisk();
        }

        void IEventListener.OnNotify(int eventCode, object context)
        {
            if(eventCode == Events.ON_CLONE_COMPLETE)
            {
                GitCloneRequest request = (GitCloneRequest)context;
                AtomPackage package = LoadPackageFromDirectory(request.workingDirectory);
				if(package == null)
				{
					Debug.Log("Could Not Parse");
					Atom.Notify(Events.ERROR_CANT_PARSE_ATOM_FILE, request.workingDirectory);
				}
				else
				{
					package.contentURL = request.sourceURL;
				}
            }
        }
    }
}
