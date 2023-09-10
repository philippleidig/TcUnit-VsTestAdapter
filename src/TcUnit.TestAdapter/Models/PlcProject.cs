using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
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

        private PlcProject(string plcProjectFile)
        {
            CompletePathInFileSystem = plcProjectFile;
            FolderPathInFileSystem = Path.GetDirectoryName(plcProjectFile);
            FileNameInFileSystem = new DirectoryInfo(plcProjectFile).Name.ToString();
            Name = FileNameInFileSystem.Replace(".plcproj", "");

            ParsePOUs();
        }

        public static PlcProject ParseFromProjectFile(string plcProjectFile)
        {
            if (!File.Exists(plcProjectFile))
            {
                throw new FileNotFoundException();
            }

            if(!plcProjectFile.Contains(".plcproj"))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new PlcProject(plcProjectFile);
        }

        public static XNamespace XmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private void ParsePOUs ()
        {
            var doc = XDocument.Load(CompletePathInFileSystem);



            var nodes = doc.Elements(XmlNamespace + "Project")
                            .Elements(XmlNamespace + "ItemGroup")
                            .Elements(XmlNamespace + "Compile");

            foreach (var node in nodes)
            {
                var relativePath = node.Attribute("Include")?.Value;
                if (relativePath.Contains(".TcPOU"))
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

                var parameters = plcLibrary.Element(XmlNamespace + "Parameters");

                if (parameters == null)
                {
                    continue;
                }

                foreach (var parameter in parameters.Elements(XmlNamespace + "Parameter"))
                {
                    var key = parameter.Element(XmlNamespace + "Key")?.Value;
                    var value = parameter.Element(XmlNamespace + "Value")?.Value;
               
                    reference.Parameters.Add(key, value);
                }
           
                References.Add(reference);
            }
        }
    }
}
