﻿
using AtomPackageManager.Packages;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class CodeDomCompilerService : ICompilerService
    {
        /// <summary>
        /// The result of our compiling.
        /// </summary>
        private CompilerResults m_CompileResults;

        /// <summary>
        /// Returns if the compile was successful or not. 
        /// </summary>
        public bool wasSuccessful
        {
            get
            {
                return !m_CompileResults.Errors.HasErrors;
            }
        }

        /// <summary>
        /// Takes an atom package and then compiles it to disk.
        /// </summary>
        /// <param name="package"></param>
        public void CompilePackage(AtomPackage package, OnCompileCompleteDelegate onComplete)
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
                    string rootPath = Constants.SCRIPT_IMPORT_DIRECTORY + package.packageName;
                    //Debug.Log("Root: " + rootPath);
                    // Add the local script path
                    string scriptPath = rootPath + assembly.compiledScripts[i];
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
                m_CompileResults = codeProvider.CompileAssemblyFromFile(parameters, scriptsToCompile);
            }

            // Fire our callback
            if (onComplete != null)
            {
                onComplete(this, package);
            }

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
            return m_CompileResults.Errors;
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