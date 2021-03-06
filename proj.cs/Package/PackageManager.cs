﻿using AtomPackageManager.Packages;
using AtomPackageManager.Services;
using AtomPackageManager.Strings;
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

        [SerializeField]
        private List<AtomPackage> m_Packages = new List<AtomPackage>();

        [SerializeField]
        private AtomSettings m_Settings;


        public List<AtomPackage> packages
        {
            get { return m_Packages; }
        }

        public void Save()
        {
            // Cast us to JSON
            string json = JsonUtility.ToJson(this, true);
            // Save our settings
            m_Settings.Save();
            // Save it to disk
            File.WriteAllText(FilePaths.packageManagerPath, json);
        }

        public void Load()
        {
            // Check if it exists
            if (File.Exists(FilePaths.packageManagerPath))
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

            // Load our settings
            m_Settings.Load();
        }

        public void AddPackage(AtomPackage package)
        {
            packages.Add(package);
            Save();
        }

        public void RemovePackage(int packageIndex)
        {
            if (packageIndex > 0 && packageIndex < m_Packages.Count)
            {
                RemovePackage(m_Packages[packageIndex]);
            }
        }

        public void RemovePackage(AtomPackage package)
        {
            if(package != null && m_Packages.Contains(package))
            {
                m_Packages.Remove(package);
                Save();
            }
        }

        /// <summary>
        /// Invoked when we have a ON_CLONE_COMPLETE event.
        /// </summary>
        /// <param name="directory"></param>
        public void CloneComplete(ISourceControlService service)
        {
            if (service.wasSuccessful)
            {
                // Try to find the atom.yaml in the root
                string[] files = Directory.GetFiles(service.directory, "*.atom");

                // Do we have any results?
                if(files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        LoadAtomFileAtPath(files[i]);
                    }
                }
                else
                {
                    if( EditorUtility.DisplayDialog(CreateNewAtomFileDialog.Title,
                                                    string.Format(CreateNewAtomFileDialog.Message, service.repositoryURL),
                                                    CreateNewAtomFileDialog.OkayButton,
                                                    CreateNewAtomFileDialog.CancelButton))
                    {
                        AtomPackage newPackage = AtomPackage.CreatePackage(service.repositoryURL, service.directory);
                        m_Packages.Add(newPackage);
                        Save();
                    }
                }

                PackageEditor editor = Object.FindObjectOfType<PackageEditor>();

                if (editor != null)
                {
                    editor.RefreshValues();
                }
            }
        }

        /// <summary>
        /// Takes in a path to a .json file and if deserialize it. 
        /// </summary>
        /// <param name="assetPath"></param>
        public void LoadAtomFileAtPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new System.ArgumentNullException("assetPath", "The asset path of the file loaded was null. Can't parse an empty path");
            }

            // Get our json
            string json = File.ReadAllText(assetPath);
            // Get it's json version
            AtomPackage package = new AtomPackage();
            // Over write it
            JsonUtility.FromJsonOverwrite(json, package);
            // Add it to our lists
            m_Packages.Add(package);
        }

        /// <summary>
        /// Checks the path to see if it is managed by Atom. Returns true if it is and false if it 
        /// does not.
        /// </summary>
        public bool IsAtomAssembly(string assetPath)
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
                    if (string.CompareOrdinal(assetPath, assembly.unityAssetPath + assembly.assemblyName + ".dll") == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the Atom Assembly that belongs at the set asset path and null
        /// if one could not be found. 
        /// </summary>
        public AtomAssembly GetAssemblyByPath(string assetPath)
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
                    if (string.CompareOrdinal(assetPath, assembly.unityAssetPath + assembly.assemblyName + ".dll") == 0)
                    {
                        return assembly;
                    }
                }
            }
            return null;
        }
    }
}
