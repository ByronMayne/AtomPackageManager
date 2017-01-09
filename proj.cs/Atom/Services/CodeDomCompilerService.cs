
using AtomPackageManager.Packages;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AtomPackageManager.Services
{
    public class CodeDomCompilerService : Object, IEventListener
    {
        private void CompileAtomPackage(AtomPackage package)
        {
            // Loop over all assemblies
            foreach(AtomAssembly assembly in package.assemblies)
            {
                string directory = Path.GetDirectoryName(assembly.systemAssetPath);

                // Validate our output detestation exists. 
                if(!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create a new array
                string[] scriptsToCompile = new string[assembly.compiledScripts.Count];
                // Make our paths
                for(int i = 0; i < scriptsToCompile.Length; i++)
                {
                    // Build our path
                    string rootPath = Path.Combine(Atom.scriptImportLocation, package.name);
                    //Debug.Log("Root: " + rootPath);
                    // Add the local script path
                    string scriptPath = Path.Combine(rootPath, assembly.compiledScripts[i]);
                    //Debug.Log("Script Path: " + assembly.compiledScripts[i]);
                    // Set our path.
                    scriptsToCompile[i] = scriptPath;
                }

                // Create our provider options
                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                // Add our compiler version
                providerOptions.Add("CompilerVersion", "v3.5");
                // Create our provider
                CSharpCodeProvider codeProvider = new CSharpCodeProvider(providerOptions);
                // Setup our parameters.
                CompilerParameters parameters = new CompilerParameters();
                // We are not making an exe but a dll.
                parameters.GenerateExecutable = false;
                // Where we are outputting
                parameters.OutputAssembly = assembly.systemAssetPath + assembly.assemblyName + ".dll";
                // If we should be dubug symbols
                parameters.IncludeDebugInformation = true; // TODO an option
                // We want UnityEngine
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<MonoBehaviour>());
                // and System
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<System.Action>());
                // And the editor
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<UnityEditor.Editor>());
                // We don't want to load in memory
                parameters.GenerateInMemory = false;
                // Set our warning level
                parameters.WarningLevel = 3;
                // And if we should treat warnings as errors
                parameters.TreatWarningsAsErrors = false;
                // Create a new results object
                Debug.Log(Time.timeSinceLevelLoad);
                CompilerResults compileResults = codeProvider.CompileAssemblyFromFile(parameters, scriptsToCompile);
                Debug.Log(Time.timeSinceLevelLoad);
                // Print our errors.
                compileResults.Errors.Cast<CompilerError>().ToList().ForEach(error => UnityEngine.Debug.LogError(error.ErrorText));
                // Send our on complete event
                Atom.Notify(Events.COMPILE_COMPLETE, assembly.systemAssetPath);
            }
        }

        /// <summary>
        /// Takes in a type and gets it's assembly location. 
        /// </summary>
        /// <typeparam name="T">The type you want to look for the assembly of</typeparam>
        /// <returns>The location on disk of the assembly.</returns>
        private string GetAssemblyLocation<T>()
        {
            return typeof(T).Assembly.Location;
        }


        void IEventListener.OnNotify(int eventCode, object context)
        {
            if(eventCode == Events.COMPILE_PACKAGE_REQUEST)
            {
                // Cast our type
                AtomPackage package = context as AtomPackage;
                // Handle errors
                if(package != null)
                {
                    CompileAtomPackage(package);
                }
                else
                {
                    Atom.Notify(Events.ERROR_INVALID_CAST_FOR_COMPILER, "The cast failed for the CodeDomCompilerService as it is not a AtomPackage");
                }
            }
        }
    }
}
