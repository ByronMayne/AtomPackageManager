using AtomPackageManager.Packages;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AtomPackageManager.Services.Implementations
{
    public class ProjectCreator
    {
        public static void CreateCSProject(AtomPackage package, int assemblyIndex, string[] resolvedReferences)
        {
            AtomAssembly assembly = package.assemblies[assemblyIndex];

            if (!assembly.addToProjectSolution)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                string outputPath = FilePaths.generatedProjectsDirectory + assembly.assemblyName + ".csproj";
                XmlWriter xmlWriter = XmlWriter.Create(outputPath, settings);
                xmlWriter.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
                xmlWriter.WriteAttributeString("ToolsVersion", "14.0");
                xmlWriter.WriteAttributeString("DefaultTargets", "Build");
                {
                    xmlWriter.WriteStartElement("PropertyGroup");
                    xmlWriter.WriteStartElement("ProjectGuid");
                    xmlWriter.WriteString(System.Guid.NewGuid().ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }

                {
                    xmlWriter.WriteStartElement("Import");
                    xmlWriter.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
                    xmlWriter.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
                    xmlWriter.WriteEndElement();
                }

                // References
                {
                    xmlWriter.WriteStartElement("ItemGroup");
                    for (int i = 0; i < resolvedReferences.Length; i++)
                    {
                        xmlWriter.WriteStartElement("Reference");
                        xmlWriter.WriteAttributeString("Include", resolvedReferences[i]);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                // Scripts            
                {
                    xmlWriter.WriteStartElement("ItemGroup");
                    for (int i = 0; i < assembly.compiledScripts.Count; i++)
                    {
                        xmlWriter.WriteStartElement("Compile");
                        xmlWriter.WriteAttributeString("Include", "../" + package.packageName + "/" + assembly.compiledScripts[i]);
                        xmlWriter.WriteElementString("Link", Path.GetFileName(assembly.compiledScripts[i]));
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
        }
    }
}
