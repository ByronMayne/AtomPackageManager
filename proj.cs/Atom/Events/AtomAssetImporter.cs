
using UnityEditor;

namespace AtomPackageManager
{
    public class AtomAssetImporter : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets,
                                          string[] deletedAssets,
                                          string[] movedAssets,
                                          string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                // Try to find plug in importers
                PluginImporter importer = AssetImporter.GetAtPath(importedAssets[i]) as PluginImporter;

                if(importer != null)
                {
                    Atom.Notify(Events.PLUGIN_IMPORTED, importer);
                }
            }
        }
    }
}
