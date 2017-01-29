
using AtomPackageManager.Packages;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using AtomPackageManager.Resolvers;
using System.Threading;
using System.Collections;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class CodeDomCompilerService : ThreadRoutine, ICompilerService
    {
        /// <summary>
        /// The result of our compiling.
        /// </summary>
        private CompilerErrorCollection m_CompileResults;

        private AtomPackage m_Package;
        private PackageManager m_PackageManager;
        private OnCompileCompleteDelegate m_OnComplete;
        private bool m_IsComplete;

        /// <summary>
        /// Returns if the compile was successful or not. 
        /// </summary>
        public bool wasSuccessful
        {
            get
            {
                return !m_CompileResults.HasErrors;
            }
        }

        public void CompilePackage(AtomPackage package, PackageManager packageManager, OnCompileCompleteDelegate onComplete)
        {
            // Assign our locals
            m_Package = package;
            m_PackageManager = packageManager;
            m_OnComplete = onComplete;
            StartThread();
        }

        protected override void OnOperationStarted()
        {
            EditorApplication.LockReloadAssemblies(); 
        }

        protected override IEnumerator<RoutineInstructions> ProcessOperation()
        {
            // Create new error collection
            m_CompileResults = new CompilerErrorCollection();

            // Loop over all assemblies
            foreach (AtomAssembly assembly in m_Package.assemblies)
            {
                string directory = Path.GetDirectoryName(assembly.systemAssetPath);

                // Validate our output detestation exists. 
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create a new array
                string[] scriptsToCompile = new string[assembly.compiledScripts.Count];
                // Make our paths
                for (int i = 0; i < scriptsToCompile.Length; i++)
                {
                    // Build our path
                    string rootPath = Constants.SCRIPT_IMPORT_DIRECTORY + m_Package.packageName;
                    //Debug.Log("Root: " + rootPath);
                    // Add the local script path
                    string scriptPath = rootPath + Path.DirectorySeparatorChar + assembly.compiledScripts[i];
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
                // If we should be debug symbols
                parameters.IncludeDebugInformation = true; // TODO an option
                // Get our reference names (this is only the 'UnityENgine' instead of the system path which we need
                IList<string> referencedAssemblies = assembly.references;
                // Create our return 
                string[] resolvedAssemblyPaths = null;

                // Check if we have to yield on other processes
                yield return RoutineInstructions.ContinueOnMainThread;

                // Use the assembly resolver
                resolvedAssemblyPaths = AssemblyReferenceResolver.ResolveAssemblyPaths(assembly, m_PackageManager);

                // Check if we have to yield on other processes
                yield return RoutineInstructions.ContinueOnThread;

                // Make sure there was no error
                if (resolvedAssemblyPaths == null || resolvedAssemblyPaths.Length == 0)
                {
                    continue;
                }
                // Add them to our compiler
                parameters.ReferencedAssemblies.AddRange(resolvedAssemblyPaths);
                // We want UnityEngine
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<MonoBehaviour>());
                // and System
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<Action>());
                // And the editor
                parameters.ReferencedAssemblies.Add(GetAssemblyLocation<Editor>());
                // We don't want to load in memory
                parameters.GenerateInMemory = false;
                // Set our warning level
                parameters.WarningLevel = 3;
                // And if we should treat warnings as errors
                parameters.TreatWarningsAsErrors = false;
                // Create a new results object
                var results = codeProvider.CompileAssemblyFromFile(parameters, scriptsToCompile);
                // Add to our global errors.
                m_CompileResults.AddRange(results.Errors);
            }
            m_IsComplete = true;
        }

        protected override void OnOperationComplete()
        {
            // Fire our callback
            if (m_OnComplete != null)
            {
                m_OnComplete(this, m_Package);
            }
            // Now we can reload
            EditorApplication.UnlockReloadAssemblies();
        }

        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        public ICompilerService CreateCopy()
        {
            return MemberwiseClone() as ICompilerService;
        }

        /// <summary>
        /// Returns back all the errors (if any) from our compiling. 
        /// </summary>
        public CompilerErrorCollection GetErrors()
        {
            return m_CompileResults;
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
    }
}
