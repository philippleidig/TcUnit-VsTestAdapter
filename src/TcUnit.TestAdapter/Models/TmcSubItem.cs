using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcSubItem : TmcItem
    {
        public string Type { get; set; }

        internal TmcSubItem(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public static TmcSubItem Parse(XElement element)
        {
            string name = element.Element("Name").Value;
            string type = element.Element("Type").Value;


            return new TmcSubItem(name, type);
        }
    }
}
