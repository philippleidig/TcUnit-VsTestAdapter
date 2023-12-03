using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TcUnit.TestAdapter.XmlExtensions;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter.Models
{
    public class PlcProject
    {
        public string CompletePathInFileSystem { get; set; }
        public string FolderPathInFileSystem { get; set; }
        public string FileNameInFileSystem { get; set; }
        public string Name { get; set; }

        public List<FunctionBlock_POU> FunctionBlocks { get; set; } = new List<FunctionBlock_POU>();

        public List<PlcLibraryReference> References { get; set; } = new List<PlcLibraryReference> { };

        public List<TwinCATModuleClass> ModuleClasses { get; set; } = new List<TwinCATModuleClass> { };



        private PlcProject(string plcProjectFile)
        {
            CompletePathInFileSystem = plcProjectFile;
            FolderPathInFileSystem = Path.GetDirectoryName(plcProjectFile);
            FileNameInFileSystem = new DirectoryInfo(plcProjectFile).Name.ToString();
            Name = FileNameInFileSystem.Replace(".plcproj", "");

            ParsePOUs();
            ParseTMCs();
        }

        public static PlcProject ParseFromProjectFile(string plcProjectFile)
        {
            if (!File.Exists(plcProjectFile))
            {
                throw new FileNotFoundException();
            }

            if (!plcProjectFile.Contains(".plcproj"))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new PlcProject(plcProjectFile);
        }

        public static XNamespace XmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";


        private void ParseTMCs()
        {
            var doc = XDocument.Load(CompletePathInFileSystem);

            var nodes = doc.Elements(XmlNamespace + "Project")
                            .Elements(XmlNamespace + "ItemGroup")
                            .Elements(XmlNamespace + "None");

            foreach (var node in nodes)
            {
                var relativePath = node.Attribute("Include")?.Value;
                if (relativePath.EndsWith(".tmc"))
                {
                    var tmcFilePath = Path.Combine(FolderPathInFileSystem, relativePath);

                    if (File.Exists(tmcFilePath))
                    {
                        var tmc = TwinCATModuleClass.ParseFromFilePath(tmcFilePath);
                        ModuleClasses.Add(tmc);
                    }
                }
            }
        }

        private void ParsePOUs()
        {
            var doc = XDocument.Load(CompletePathInFileSystem);

            var nodes = doc.Elements(XmlNamespace + "Project")
                            .Elements(XmlNamespace + "ItemGroup")
                            .Elements(XmlNamespace + "Compile");

            foreach (var node in nodes)
            {
                var relativePath = node.Attribute("Include")?.Value;
                if (relativePath.EndsWith(".TcPOU"))
                {
                    var pouFilePath = Path.Combine(FolderPathInFileSystem, relativePath);

                    if (File.Exists(pouFilePath))
                    {
                        var functionBlock = FunctionBlock_POU.ParseFromFilePath(pouFilePath);
                        FunctionBlocks.Add(functionBlock);
                    }
                }
            }

            var plcReferences = doc.Elements(XmlNamespace + "Project")
                                            .Elements(XmlNamespace + "ItemGroup")
                                            .Elements(XmlNamespace + "PlaceholderReference");

            foreach (var plcLibrary in plcReferences)
            {
                var name = plcLibrary.Attribute("Include")?.Value;

                var reference = new PlcLibraryReference
                {
                    Name = name,
                };

                var parameters = plcLibrary.ElementAnyNS("Parameters");

                if (parameters == null)
                {
                    continue;
                }

                foreach (var parameter in parameters.ElementsAnyNS("Parameter"))
                {
                    var key = parameter.ElementAnyNS("Key")?.Value;
                    var value = parameter.ElementAnyNS("Value")?.Value;

                    reference.Parameters.Add(key, value);
                }

                References.Add(reference);
            }
        }
    }
}
