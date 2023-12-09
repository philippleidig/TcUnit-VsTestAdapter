using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcDataType : TmcItem
    {
        public string ExtendsType { get; set; }

        internal TmcDataType(string name, string extendsType = null)
        {
            Name = name;
            ExtendsType = extendsType;
        }

        public static TmcDataType Parse(XElement element)
        {
            string name = element.Element("Name").Value;

            string extendsTypeName = null;
            var extendsType = element.Element("ExtendsType");
            if (extendsType != null)
            {
                string ns = extendsType.Attribute("Namespace")?.Value ?? "";
                extendsTypeName = ns + "." + extendsType.Value;
            }

            var dataType = new TmcDataType(name, extendsTypeName);

            return dataType;
        }
    }
}
