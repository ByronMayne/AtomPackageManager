using AtomPackageManager.Packages;
using UnityEditor;

namespace AtomPackageManager.Services
{
    public interface IPluginImporterService
    {
        void ApplyAtomImporterSettings(PluginImporter importer, AtomAssembly assembly);

        /// Creates a shallow copy of this object
        /// </summary>
        IPluginImporterService CreateCopy();
    }
}
