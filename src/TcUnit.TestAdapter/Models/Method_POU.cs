using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class Method_POU : POU
    {
        internal Method_POU(string name, Guid id, string declaration, string implementation)
        {
            Name = name;
            Id = id;
        }

        public static Method_POU Parse(XElement xMethod)
        {
            string name = xMethod.Attribute("Name").Value;
            Guid id = Guid.Parse(xMethod.Attribute("Id").Value);
            string declaration = xMethod.Element("Declaration").Value;
            string implementation = xMethod.Element("Implementation").Element("ST").Value;

            var method = new Method_POU(name, id, declaration, implementation);

            return method;
        }
    }
}
