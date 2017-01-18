using AtomPackageManager.Packages;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class PluginImporterService : IPluginImporterService
    {
        public void ApplyAtomImporterSettings(AtomAssembly assembly)
        {
            // Get our importer at that path
            PluginImporter importer = AssetImporter.GetAtPath(assembly.unityAssemblyPath) as PluginImporter;

            // If it's not null apply the settings
            if(importer != null)
            {
                bool hadChanges = assembly.supportedPlatforms.ApplyToImporter(importer);

                if (hadChanges)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        public IPluginImporterService CreateCopy()
        {
            return MemberwiseClone() as IPluginImporterService;
        }
    }
}
