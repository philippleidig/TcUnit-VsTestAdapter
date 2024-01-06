using System;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class Method_POU : POU
    {
        private Method_POU(string name, Guid id, XElement implementation)
        {
            Name = name;
            Implementation = new StructuredTextImplementation(implementation);
        }

        public static Method_POU Parse(XElement element)
        {
            string name = element.Attribute("Name").Value;
            Guid id = Guid.Parse(element.Attribute("Id").Value);
            var implementation = element.Element("Implementation").Element("ST");

            if (implementation == null)
            {
                throw new NotSupportedException("Only structured text implementation is supported.");
            }

            return new Method_POU(name, id, implementation);
        }
    }
}
