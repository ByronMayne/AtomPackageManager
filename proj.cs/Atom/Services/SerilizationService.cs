using UnityEngine;
using AtomPackageManager.Packages;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace AtomPackageManager.Services
{
    public class SerilizationService : Object, IEventListener
    {
        [MenuItem("Atom/Create Default")]
        internal static void CreateDefaultPackgeFile()
        {
            // create the new request
            SerilizationRequest request = new SerilizationRequest();
            // Create the package
            AtomPackage package = ScriptableObject.CreateInstance<AtomPackage>();
            AtomAssembly assembly = new AtomAssembly();
            assembly.assemblyName = "Example";
            assembly.supportedPlatforms.EditorCompatible = true;
            assembly.compatibleWithAnyPlatform = true;
            assembly.editorCompatible = true;
            assembly.compiledScripts = new List<string>()
                {
                    "Assets/Scripts/Player.cs",
                    "Assets/Scripts/Weapons/Gun.cs",
                    "Assets/Scripts/Weapons/Bomb.cs",
                };
            assembly.references = new List<string>()
            {
                typeof(MonoBehaviour).Assembly.Location,
                typeof(System.Action).Assembly.Location
            };

            package.assemblies = new List<AtomAssembly>() { assembly };

            // Assign it's value
            request.serializedDataPath = Application.dataPath + "/Example.atom";
            request.package = package;
            Atom.Notify(Events.SERIALIZATION_REQUEST, request);
        }

        private void DeserializePackage(SerilizationRequest request)
        {
            // Create a new one
            AtomPackage package = ScriptableObject.CreateInstance<AtomPackage>();
            // Fill it
            string json = File.ReadAllText(request.serializedDataPath);
            // Load it
            JsonUtility.FromJsonOverwrite(json, package);
            // Send the event
            Atom.Notify(Events.DESERIALIZATION_COMPLETE, package);
        }

        private void SerializePackage(SerilizationRequest request)
        {
            // Write json
            string json = JsonUtility.ToJson(request.package);
            // Save it to disk
            File.WriteAllText(request.serializedDataPath, json);
            // Fire the event
            Atom.Notify(Events.SERIALIZATION_COMPLETE, request);
        }

        public void OnNotify(int eventCode, object context)
        {
            if(eventCode == Events.DESERIALIZATION_REQUEST)
            {
                Debug.Log("DESERIALIZATION_REQUEST");
                // Get our request
                SerilizationRequest request = (SerilizationRequest)context;
                // Get the package
                DeserializePackage(request);
            }

            else if ( eventCode == Events.SERIALIZATION_REQUEST )
            {
                Debug.Log("SERIALIZATION_REQUEST");
                // Get our request
                SerilizationRequest request = (SerilizationRequest)context;
                // Get the package
                SerializePackage(request);
            }
        }
    }
}
