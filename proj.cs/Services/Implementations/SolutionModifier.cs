using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using AtomPackageManager.Packages;

namespace AtomPackageManager.Services
{

    public class SolutionModifier : ISolutionModifier
    {
        /// <summary>
        /// Invoked when Unity has generated our solution. 
        /// </summary>
        /// <param name="solutionPath">The path on disk to the solution</param>
        public void ModifySolution(string solutionPath, PackageManager packageManager)
        {
            // Make sure we have a valid path
            Assert.IsFalse(string.IsNullOrEmpty(solutionPath), "The solution path was null or empty");
            // The file must exist
            Assert.IsTrue(File.Exists(solutionPath), "No solution exists at that requested path: " + solutionPath);
            // It must have the correct extension
            Assert.IsTrue(Path.GetExtension(solutionPath) == Constants.SOLUTION_EXTENSION, "An invalid file was sent to be modified " + solutionPath + " does not end with " + Constants.SOLUTION_EXTENSION);
            // Create a string builder for our new contents
            StringBuilder builder = new StringBuilder();
            // Open the file for reading
            using (StreamReader reader = new StreamReader(solutionPath))
            {

                bool foundProjectHeader = false;
                bool writeComplete = false;
                List<string> projectsInSolution = new List<string>();
                while (!reader.EndOfStream)
                {
                    // Get the first line
                    string line = reader.ReadLine();

                    if (!writeComplete)
                    {

                        if (line.StartsWith("Project("))
                        {
                            // We found our header
                            foundProjectHeader = true;

                            PersistenceBlock reference = new PersistenceBlock();
                            reference.ParseFromString(line);

                            // Add the name to our list
                            projectsInSolution.Add(reference.name);
                        }
                        else if (foundProjectHeader && !line.StartsWith("EndProject"))
                        {
                            for(int i = 0; i < packageManager.packages.Count; i++)
                            {
                                // Get our current
                                AtomPackage current = packageManager.packages[i];
                                // Check if it's include
                                foreach(AtomAssembly assembly in packageManager.packages[i].assemblies)
                                {
                                    if (!projectsInSolution.Contains(assembly.assemblyName))
                                    {
                                        // It's not there so we make a new one
                                        PersistenceBlock reference = new PersistenceBlock();
                                        reference.name = current.packageName;
                                        reference.path = FilePaths.generatedProjectsDirectory + assembly.assemblyName + ".csproj";
                                        reference.projectGUID = System.Guid.NewGuid().ToString();
                                        writeComplete = true;
                                        builder.AppendLine(reference.ToString());
                                        builder.AppendLine("EndProject");
                                        projectsInSolution.Add(current.packageName);
                                    }
                                }
                            }
                        }
                    }

                    // Push the contents to our builder. 
                    builder.AppendLine(line);
                }
            }

            // Now write it
            File.WriteAllText(solutionPath, builder.ToString());
        }

        /// Creates a shallow copy of this object
        /// </summary>
        public ISolutionModifier CreateCopy()
        {
            return MemberwiseClone() as ISolutionModifier;
        }
    }
}
