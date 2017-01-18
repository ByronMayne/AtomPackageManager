using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AtomPackageManager.Resolvers
{
    public class AssemblyReferenceResolver
    {
        public static string[] ResolveAssemblyPaths(IList<string> assemblyNames)
        {
            // Create a new array with the same number of elements
            string[] resolvedAssemblyPaths = new string[assemblyNames.Count];

            // A few quick shorthands
            for (int i = 0; i < assemblyNames.Count; i++)
            {
                // Get our current for convince. 
                string assemblyName = assemblyNames[i];
                string assemblyLocation = string.Empty;

                // Check for UnityEditor.
                if (IsAssemblyOfType<UnityEngine.Object>(assemblyName, ref assemblyLocation))
                {
                    resolvedAssemblyPaths[i] = assemblyLocation;
                    continue;
                }

                // Check for UnityEngine
                if (IsAssemblyOfType<UnityEngine.Object>(assemblyName, ref assemblyLocation))
                {
                    resolvedAssemblyPaths[i] = assemblyLocation;
                    continue;
                }

                // Check for System.
                if (IsAssemblyOfType<System.Object>(assemblyName, ref assemblyLocation))
                {
                    resolvedAssemblyPaths[i] = assemblyLocation;
                    continue;
                }

                // Check for System.XML
                if (IsAssemblyOfType<System.Xml.XmlAttribute>(assemblyName, ref assemblyLocation))
                {
                    resolvedAssemblyPaths[i] = assemblyLocation;
                    continue;
                }

                // Now search our App Domain (which is slower)
                resolvedAssemblyPaths[i] = FindAssemblyInAppDomain(assemblyName);

                // Check to see if it was resolved
                if(string.IsNullOrEmpty(resolvedAssemblyPaths[i]))
                {
                    string error = "Assembly Resolver Error: Unable to resolve assembly '" + assemblyName + "'.";
                    error += System.Environment.NewLine;
                    error += "Make sure you are using the full assembly name for example UnityEngine is '" + typeof(UnityEngine.Object).Assembly.FullName + "'.";
                    error += System.Environment.NewLine;
                    throw new UnresolvedAssemblyException(error);
                }
            }

            // Return the result
            return resolvedAssemblyPaths;
        }

        /// <summary>
        /// Loops over all our current assemblies and checks to see
        /// if we can find the one request. 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static string FindAssemblyInAppDomain(string assemblyName)
        {
            // Get our current
            AppDomain currentDomain = AppDomain.CurrentDomain;
            // Get our current list of assemblies (this can be huge). 
            Assembly[] assemblies = currentDomain.GetAssemblies();
            // Now we must loop over them all
            for (int x = 0; x < assemblies.Length; x++)
            {
                // Do the names match?
                if (string.CompareOrdinal(assemblyName, assemblies[x].FullName) == 0)
                {
                    return assemblies[x].Location;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Checks to see if the typeof(T)'s assembly has a name that matches what we are looking for. 
        /// </summary>
        public static bool IsAssemblyOfType<T>(string assemblyName, ref string assemblyLocation)
        {
            // Get our assembly
            Assembly assembly = typeof(T).Assembly;
            // Get the name

            // Is it the Unity Engine Assembly?
            if (string.CompareOrdinal(assemblyName, assembly.FullName) == 0)
            {
                // Keep it's location
                assemblyLocation = assembly.Location;
                // Return
                return true;
            }
            return false;
        }
    }
}
