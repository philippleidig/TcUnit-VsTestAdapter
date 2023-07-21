using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.Models
{
    public class PlcProject
    {
        public string CompletePathInFileSystem { get; set; }
        public string FolderPathInFileSystem { get; set; }
        public string FileNameInFileSystem { get; set; }
        public string Name { get; set; }

        public List<FunctionBlock_POU> FunctionBlocks { get; set; } = new List<FunctionBlock_POU>();

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

        private void ParsePOUs ()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(CompletePathInFileSystem);
            XmlNodeList nodes = doc.GetElementsByTagName("Compile");

            foreach (XmlNode node in nodes)
            {
                var relativePath = node.Attributes["Include"].Value;
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
        }
    }
}
