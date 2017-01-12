
using UnityEditor;
using UnityEngine;

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
                    // Makes the request float around in the Unity runtime until Atom picks it up and deletes it.
                    ImportPluginRequest import = ImportPluginRequest.CreateInstance<ImportPluginRequest>();
                    import.importer = importer;
                }
            }
        }
    }
}
