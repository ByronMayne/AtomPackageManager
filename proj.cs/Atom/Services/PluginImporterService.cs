using AtomPackageManager.Packages;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Services
{
    public class PluginImporterService : Object, IEventListener
    {
        private void ApplyAtomImporterSettings(ImportPluginRequest importPluginRequest)
        {
            PluginImporter importer = importPluginRequest.importer;
            AtomAssembly assembly = importPluginRequest.assembly;

            Debug.Log("Importing");
            bool hadChanges = assembly.supportedPlatforms.ApplyToImporter(importer);

            if(hadChanges)
            {
                importer.SaveAndReimport();
            }
        }

        public void OnNotify(int eventCode, object context)
        {
            if(eventCode == Events.PLUGIN_SET_IMPORTER_SETTINGS_REQUEST)
            {
                ApplyAtomImporterSettings((ImportPluginRequest)context);
            }
        }
    }
}
