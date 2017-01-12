using AtomPackageManager.Packages;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class PluginImporterService : IPluginImporterService
    {
        public void ApplyAtomImporterSettings(PluginImporter importer, AtomAssembly assembly)
        {
            bool hadChanges = assembly.supportedPlatforms.ApplyToImporter(importer);

            if(hadChanges)
            {
               importer.SaveAndReimport();
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
