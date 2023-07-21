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


        internal Method_POU(string name, string dataType, Guid id, string declaration, string implementation)
        {
            Name = name;
            ReturnDataType = dataType;
            ACCESS = Accessor.NONE;
            Id = id;
        }

        public Accessor ACCESS { get; set; }

        public string ReturnDataType { get; set; }

        public static Method_POU Parse(XElement element)
        {
            string name = element.Attribute("Name").Value;
            Guid id = Guid.Parse(element.Attribute("Id").Value);
            string declaration = element.Element("Declaration").Value;
            string implementation = element.Element("Implementation").Element("ST").Value;

            string dataType = declaration.Split(new char[]
            {
                    ':'
            }, StringSplitOptions.RemoveEmptyEntries).Last<string>().Trim().Split(new char[]
            {
                    ' '
            }).First<string>();

            var method = new Method_POU(name, dataType, id, declaration, implementation);

            return method;
        }
    }
}
