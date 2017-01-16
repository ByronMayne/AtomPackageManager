using AtomPackageManager.Packages;
using UnityEditor;

namespace AtomPackageManager.Services
{
    public interface IPluginImporterService
    {
        void ApplyAtomImporterSettings(AtomAssembly assembly);

        /// Creates a shallow copy of this object
        /// </summary>
        IPluginImporterService CreateCopy();
    }
}
