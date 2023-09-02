using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class FunctionBlock_POU : POU
    {

        public Dictionary<string, Method_POU> Methods = new Dictionary<string, Method_POU>();

        public string Extends { get; set; }

        public FunctionBlock_POU ()
        {
            
        }

        public static FunctionBlock_POU ParseFromFilePath(string filepath)
        {
            if(!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            if(!filepath.Contains(".TcPOU"))
            {
                throw new ArgumentOutOfRangeException();
            }

            var functionBlock = new FunctionBlock_POU();

            var file = Path.GetFileName(filepath);

            functionBlock.Name = file.Replace(".TcPOU", "");
            functionBlock.FilePath = filepath;

            XElement xelement = XDocument.Load(filepath, LoadOptions.SetLineInfo).Element("TcPlcObject").Elements("POU").First<XElement>();

            functionBlock.Id = Guid.Parse(xelement.Attribute("Id").Value);

            var declaration = xelement.Element("Declaration").Value;

            foreach (XElement xelement2 in xelement.Elements("Method"))
            {
                var method = Method_POU.Parse(xelement2);
                functionBlock.Methods.Add(method.Name, method);
            }

            return functionBlock;
        }
    }
}
