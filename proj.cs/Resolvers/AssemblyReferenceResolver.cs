using AtomPackageManager.Packages;
using AtomPackageManager.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;


namespace AtomPackageManager.Resolvers
{
    public class AssemblyReferenceResolver
    {
        public static string[] ResolveAssemblyPaths(AtomAssembly assembly, PackageManager packageManger)
        {
            // Create a new array with the same number of elements
            string[] resolvedAssemblyPaths = new string[assembly.references.Count];

            // A few quick shorthands
            for (int i = 0; i < assembly.references.Count; i++)
            {
                // Get our current for convince. 
                string assemblyName = assembly.references[i];
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
                if (IsAssemblyOfType<object>(assemblyName, ref assemblyLocation))
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

                // Did we find the assembly or just return a guess.
                bool foundAssembly = false;

                // Now search our App Domain (which is slower)
                resolvedAssemblyPaths[i] = FindAssemblyInAppDomain(assemblyName, assembly.assemblyName, ref foundAssembly);

                if (!foundAssembly)
                {
                    // Check if Atom has that assembly listed
                    resolvedAssemblyPaths[i] = FindAssemblyInAtom(packageManger, assembly.assemblyName, assemblyName, ref foundAssembly);
                    if(foundAssembly)
                    {

                    }
                    else if( string.IsNullOrEmpty(resolvedAssemblyPaths[i]))
                    {
                        if (ShowSuggestDialog(assemblyName, resolvedAssemblyPaths[i]))
                        {
                            // They want us to fix it. 
                            assembly.references[i] = resolvedAssemblyPaths[i];
                        }
                        else
                        {
                            ShowUnableToResolveAssembliy(assemblyName);
                            // They don't want to fix it. 
                            return null;
                        }
                    }
                    // Did we even have a guess?
                    else if (string.IsNullOrEmpty(resolvedAssemblyPaths[i]))
                    {
                        ShowUnableToResolveAssembliy(assemblyName);
                        return null;
                    }
                    else
                    {
                        if (ShowSuggestDialog(assemblyName, resolvedAssemblyPaths[i]))
                        {
                            // They want us to fix it. 
                            assembly.references[i] = resolvedAssemblyPaths[i];
                        }
                        else
                        {
                            ShowUnableToResolveAssembliy(assemblyName);
                            // They don't want to fix it. 
                            return null;
                        }
                    }
                }
            }

            // Return the result
            return resolvedAssemblyPaths;
        }

        /// <summary>
        /// Show the user a window asking if they want to substitute. 
        /// </summary>
        /// <returns></returns>
        private static bool ShowSuggestDialog(string userAssembly, string suggestionAssembly)
        {
            // This will always be on the same thread. 
            // Ask the user if our guess is correct.
            string message = "We were unable to resolve the assembly '" + userAssembly +
                             "'. However we found a similar named assembly '" + suggestionAssembly +
                             "'. Was this the assembly you were trying to reference?";

            return EditorUtility.DisplayDialog("Assembly Resolver Error", message, "Yes", "No");
        }

        /// <summary>
        /// Loops over Atom to try to find a package that matches our assembly name. 
        /// </summary>
        private static string FindAssemblyInAtom(PackageManager packageManger, string buildingAssembly, string assemblyName, ref bool foundAssembly)
        {
            string bestGuess = null;
            foreach (AtomPackage package in packageManger.packages)
            {
                foreach (AtomAssembly assembly in package.assemblies)
                {
                    string fullName = assembly.assemblyName + ", Version=" + package.version + " Culture=neutral, PublicKeyToken=null";

                    if (string.CompareOrdinal(assemblyName, fullName) == 0)
                    {
                        foundAssembly = true;
                        return assembly.systemAssetPath + assembly.assemblyName + ".dll";
                    }

                    string lowercase = assembly.assemblyName.ToLower();

                    if (lowercase.Contains(assemblyName.ToLower()) && !assemblyName.StartsWith(buildingAssembly + ","))
                    {
                        bestGuess = fullName;
                    }

                }
            }
            foundAssembly = false;
            return bestGuess;
        }

        private static void ShowUnableToResolveAssembliy(string assemblyName)
        {
            // Try to guess which one they were referencing (just incase they only had the temp path).
            string error = "Unable to resolve assembly '" + assemblyName + "'.";
            error += System.Environment.NewLine;
            error += "Make sure you are using the full assembly name for example UnityEngine is '" + typeof(UnityEngine.Object).Assembly.FullName + "'.";
            MessagePopup.ShowSimpleMessage("Unable to Resolve Assembly", error, MessagePopup.Type.Error);
        }

        /// <summary>
        /// Loops over all our current assemblies and checks to see
        /// if we can find the one request. 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static string FindAssemblyInAppDomain(string assemblyName, string buildingAssembly, ref bool foundAssembly)
        {
            // Get our current
            AppDomain currentDomain = AppDomain.CurrentDomain;
            // Get our current list of assemblies (this can be huge). 
            Assembly[] assemblies = currentDomain.GetAssemblies();
            // In an attempt to help the user we are going to to guess their assembly incase they are missing the full path.
            string bestGuess = string.Empty;
            // lower case for guessing
            string lowercaseAssemblyName = assemblyName.ToLower();
            // Now we must loop over them all
            for (int x = 0; x < assemblies.Length; x++)
            {
                // Do the names match?
                if (string.CompareOrdinal(assemblyName, assemblies[x].FullName) == 0)
                {
                    foundAssembly = true;
                    return assemblies[x].Location;
                }

                string lowercase = assemblies[x].FullName.ToLower();

                // We want to try to help the user. 
                if (lowercase.Contains(lowercaseAssemblyName) && !assemblyName.StartsWith(buildingAssembly + ","))
                {
                    bestGuess = assemblies[x].FullName;
                }
            }
            return bestGuess;
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
