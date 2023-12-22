using System.Collections.Generic;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcDataType : TmcItem
    {
        public string ExtendsType { get; set; }

        public List<TmcSubItem> SubItems { get; set; } = new List<TmcSubItem>();

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

            var subItems = element.Elements("SubItem");
            if (subItems != null) {
                foreach ( var subItem in subItems )
                {
                    dataType.SubItems.Add(TmcSubItem.Parse(subItem));
                }
            }

            return dataType;
        }
    }
}
