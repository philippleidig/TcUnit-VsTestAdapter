using System.Collections.Generic;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcModule : TmcItem
    {
        public List<TmcDataArea> DataAreas { get; set; } = new List<TmcDataArea>();

        public string TcSmClass { get; set; }

        internal TmcModule(string name, string tcSmClass)
        {
            Name = name;
            TcSmClass = tcSmClass;
        }

        public static TmcModule Parse(XElement element)
        {
            string name = element.Element("Name").Value;
            string tcSmClass = element.Attribute("TcSmClass").Value;

            var module = new TmcModule(name, tcSmClass);

            var dataAreas = element.Elements("DataAreas")
                                    .Elements("DataArea");
            foreach (var dataArea in dataAreas)
            {
                module.DataAreas.Add(TmcDataArea.Parse(dataArea));
            }

            return module;
        }
    }
}
