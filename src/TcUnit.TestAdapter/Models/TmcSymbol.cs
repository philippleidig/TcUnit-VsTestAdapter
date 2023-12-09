using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcSymbol : TmcItem
    {
        public string BaseType { get; set; }

        internal TmcSymbol(string name, string baseType)
        {
            Name = name;
            BaseType = baseType;
        }

        public static TmcSymbol Parse(XElement element)
        {
            string name = element.Element("Name").Value;
            string baseType = element.Element("BaseType").Value;

            var symbol = new TmcSymbol(name, baseType);

            return symbol;
        }
    }
}