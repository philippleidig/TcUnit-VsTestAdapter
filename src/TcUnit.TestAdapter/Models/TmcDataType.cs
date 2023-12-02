using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcDataType
    {
        public string Name { get; set; }
        public string FilePath { get; set; }

        public string Namespace { get; set; }

        public string ExtendsType { get; set; }

        internal TmcDataType(string name, string namespace_ = null, string extendsType = null)
        {
            Name = name;
            Namespace = namespace_;
            ExtendsType = extendsType;
    }

        public static TmcDataType Parse(XElement xMethod)
        {
            string namespace_ = xMethod.Attribute("Namespace")?.Value;
            string name = xMethod.Element("Name").Value;
            string extendsType = xMethod.Element("ExtendsType")?.Value;

            var dataType = new TmcDataType(name, namespace_, extendsType);

            return dataType;
        }
    }
}
