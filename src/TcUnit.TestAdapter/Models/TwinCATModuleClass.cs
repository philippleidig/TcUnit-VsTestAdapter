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

            var datatypes = doc.Elements(XmlNamespace + "TcModuleClass")
                            .Elements(XmlNamespace + "DataTypes")
                            .Elements(XmlNamespace + "DataType");
            foreach (var datatype in datatypes)
            {
                tmc.DataTypes.Add(TmcDataType.Parse(datatype));
            }

            var modules = doc.Elements(XmlNamespace + "TcModuleClass")
                            .Elements(XmlNamespace + "Modules")
                            .Elements(XmlNamespace + "Module");
            foreach (var module in modules)
            {
                tmc.Modules.Add(TmcModule.Parse(module));
            }

            return tmc;
        }
    }
}
