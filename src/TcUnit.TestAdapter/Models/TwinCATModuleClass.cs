using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATModuleClass
    {
        public string CompletePathInFileSystem { get; set; }
        public string FolderPathInFileSystem { get; set; }
        public string FileNameInFileSystem { get; set; }
        public string Name { get; set; }

        public List<TmcDataType> DataTypes { get; set; } = new List<TmcDataType>();

        public List<TmcModule> Modules { get; set; } = new List<TmcModule> { };


        public static XNamespace XmlNamespace = "";

        private TwinCATModuleClass()
        {

        }

        public static TwinCATModuleClass ParseFromFilePath(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            if (!filepath.EndsWith(".tmc"))
            {
                throw new ArgumentOutOfRangeException();
            }

            TwinCATModuleClass tmc = new TwinCATModuleClass();


            var doc = XDocument.Load(filepath);

            var nodes = doc.Elements(XmlNamespace + "TcModuleClass")
                            .Elements(XmlNamespace + "DataTypes")
                            .Elements(XmlNamespace + "DataType");

            foreach (var node in nodes)
            {
                tmc.DataTypes.Add(TmcDataType.Parse(node));
            }
            return tmc;
        }
    }
}
