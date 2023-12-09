using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.XmlExtensions
{
    static class XmlExtensions
    {
        public static XAttribute AttributeAnyNS<T>(this T source, string localName)
        where T : XElement
        {
            return source.Attributes().SingleOrDefault(e => e.Name.LocalName == localName);
        }
        public static XElement ElementAnyNS<T>(this T source, string localName)
        where T : XElement
        {
            return source.Elements().SingleOrDefault(e => e.Name.LocalName == localName);
        }

        public static IEnumerable<XElement> ElementsAnyNS<T>(this T source, string localName)
        where T : XElement
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }

        public static IEnumerable<XElement> ElementsAnyNS<T>(this IEnumerable<T> source, string localName)
        where T : XContainer
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }

       
    }
}
