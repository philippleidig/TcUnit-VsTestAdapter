using System.Collections.Generic;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TmcDataArea : TmcItem
    {
        public List<TmcSymbol> Symbols { get; set; } = new List<TmcSymbol>();

        internal TmcDataArea(string name)
        {
            Name = name;
        }

        public static TmcDataArea Parse(XElement element)
        {
            string name = element.Element("Name").Value;

            var dataArea = new TmcDataArea(name);

            var symbols = element.Elements("Symbol");
            foreach (var symbol in symbols)
            {
                dataArea.Symbols.Add(TmcSymbol.Parse(symbol));
            }

            return dataArea;
        }
    }
}