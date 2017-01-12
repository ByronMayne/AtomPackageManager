using AtomPackageManager.Packages;
using AtomPackageManager.Services;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace AtomPackageManager
{
    public delegate void PluginRequiresImportDeleagete(PluginImporter importer, AtomAssembly assembly);

    [System.Serializable]
    public class PackageManager
    {
        public static string GetLocationOnDisk()
        {
            return Application.dataPath.Replace("/Assets", Constants.PACKAGE_MANAGER_LOCATION);
        }

        public PackageManager(Atom atomInstance)
        {
        }

        [SerializeField]
        private List<AtomPackage> m_Packages = new List<AtomPackage>();
        public List<AtomPackage> packages
        {
            get { return m_Packages; }
        }

        public void Save()
        {
            // Cast us to JSON
            string json = JsonUtility.ToJson(this);
            // Save it to disk
            File.WriteAllText(FilePaths.packageManagerPath, json);
        }

        public void Load()
        {
            // Check if it exists
            if(File.Exists(FilePaths.packageManagerPath))
            {
                // Read the json
                string json = File.ReadAllText(FilePaths.packageManagerPath);
                // Over write this object
                JsonUtility.FromJsonOverwrite(json, this);
            }
            else
            {
                // Save our current one.
                Save();
            }
        }

        /// <summary>
        /// Invoked when we have a ON_CLONE_COMPLETE event.
        /// </summary>
        /// <param name="directory"></param>
        public void CloneComplete(ISourceControlService service)
        {
            // Try to find the atom.yaml in the root
            string[] files = Directory.GetFiles(service.workingDirectory, "*.atom");

            // Do we have any results?
            for (int i = 0; i < files.Length; i++)
            {
                // Get our json
                string json = File.ReadAllText(files[i]);
                // Get it's json version
                AtomPackage package = new AtomPackage();
                // Over write it
                JsonUtility.FromJsonOverwrite(json, package);
                // Add it to our lists
                m_Packages.Add(package);
            }

            PackageEditor editor = Object.FindObjectOfType<PackageEditor>();
            if(editor != null)
            {
                editor.LoadSerializedValues();
            }
        }

        public void ValidatePluginOwnership(PluginImporter pluginImporter, PluginRequiresImportDeleagete OnRequiresImport)
        {
            for (int i = 0; i < m_Packages.Count; i++)
            {
                // Get our current package
                AtomPackage package = m_Packages[i];
                // Loop over it's assemblies
                for (int x = 0; x < package.assemblies.Count; x++)
                {
                    // get our current assembly
                    AtomAssembly assembly = package.assemblies[x];
                    // Check if they have the same path
                    if (string.CompareOrdinal(pluginImporter.assetPath, assembly.unityAssetPath + assembly.assemblyName + ".dll") == 0)
                    {
                        if(OnRequiresImport != null)
                        {
                            OnRequiresImport(pluginImporter, assembly);
                        }
                    }
                }
            }
        }
    }
}
