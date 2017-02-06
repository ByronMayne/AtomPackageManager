
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
using AtomPackageManager.Services.Implementations;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class CodeDomCompilerService : ThreadRoutine, ICompilerService
    {
        /// <summary>
        /// The result of our compiling.
        /// </summary>
        private CompilerErrorCollection m_CompileResults;

        private AtomAssembly m_Assembly;
        private AtomPackage m_Package;
        private int m_AssemblyIndex;
        private PackageManager m_PackageManager;
        private OnCompileCompleteDelegate m_OnComplete;

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

        public void CompilePackage(AtomPackage package, int assemblyIndex, PackageManager packageManager, OnCompileCompleteDelegate onComplete)
        {
            // Assign our locals
            m_Package = package;
            m_Assembly = package.assemblies[assemblyIndex];
            m_PackageManager = packageManager;
            m_AssemblyIndex = assemblyIndex;
            m_OnComplete = onComplete;
            // Create new error collection
            m_CompileResults = new CompilerErrorCollection();
            // Loop over all assemblies
            if (!m_Assembly.isQueuedForCompile)
            {
                m_Assembly.isQueuedForCompile = true;
                m_Assembly.compilingRoutine = this;
                StartThread();
            }
            else
            {
                OnOperationComplete();
                return;
            }
        }

        protected override void OnOperationStarted()
        {
            EditorApplication.LockReloadAssemblies();
        }

        protected override IEnumerator<RoutineInstructions> ProcessOperation()
        {
            string directory = Path.GetDirectoryName(m_Assembly.systemAssetPath);

            // Validate our output detestation exists. 
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a new array
            string[] scriptsToCompile = new string[m_Assembly.compiledScripts.Count];
            // Make our paths
            for (int i = 0; i < scriptsToCompile.Length; i++)
            {
                // Build our path
                string rootPath = FilePaths.atomWorkingDirectory + m_Package.packageName;
                //Debug.Log("Root: " + rootPath);
                // Add the local script path
                string scriptPath = rootPath + Path.DirectorySeparatorChar + m_Assembly.compiledScripts[i];
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
            parameters.OutputAssembly = m_Assembly.systemAssetPath + m_Assembly.assemblyName + ".dll";
            // If we should be debug symbols
            parameters.IncludeDebugInformation = true; // TODO an option
                                                       // Get our reference names (this is only the 'UnityENgine' instead of the system path which we need
            IList<string> referencedAssemblies = m_Assembly.references;
            // Create our return 
            string[] resolvedAssemblyPaths = null;

            // Check if we have to yield on other processes
            yield return RoutineInstructions.ContinueOnMainThread;

            // Use the assembly resolver
            resolvedAssemblyPaths = AssemblyReferenceResolver.ResolveAssemblyPaths(m_Assembly, m_PackageManager);
            // Check which ones we have to compile first
            foreach (AtomPackage atomPackage in m_PackageManager.packages)
            {
                for (int assemblyIndex = 0; assemblyIndex < atomPackage.assemblies.Count; assemblyIndex++)
                {
                    foreach (string atomReferenceName in m_Assembly.references)
                    {
                        AtomAssembly current = atomPackage.assemblies[assemblyIndex];
                        if (string.CompareOrdinal(atomReferenceName, current.fullName) == 0)
                        {
                            // We have to check if it is complied. 
                            if (!current.isQueuedForCompile)
                            {
                                ICompilerService compilerService = new CodeDomCompilerService();
                                compilerService.CompilePackage(atomPackage, assemblyIndex, m_PackageManager, m_OnComplete);
                            }

                            while (!current.compilingRoutine.isComplete)
                            {
                                yield return RoutineInstructions.ContinueOnThread;
                            }
                        }
                    }
                }
            }

            yield return RoutineInstructions.ContinueOnMainThread;

            ProjectCreator.CreateCSProject(m_Package, m_AssemblyIndex, resolvedAssemblyPaths);

            // Check if we have to yield on other processes
            yield return RoutineInstructions.ContinueOnThread;


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

        protected override void OnOperationComplete()
        {
            // Fire our callback
            if (m_OnComplete != null)
            {
                m_OnComplete(this, m_Assembly);
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
