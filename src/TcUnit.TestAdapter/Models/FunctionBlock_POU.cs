using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class FunctionBlock_POU : POU
    {
        public Dictionary<string, Method_POU> Methods = new Dictionary<string, Method_POU>();
        public string Extends { get; set; }
        public List<string> AccessModifiers { get; set; } = new List<string>();
        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Implements { get; set; } = new List<string>();

        private FunctionBlock_POU ()
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

            XElement xFunctionBlock = XDocument.Load(filepath, LoadOptions.SetLineInfo).Element("TcPlcObject").Elements("POU").First<XElement>();

            functionBlock.Id = Guid.Parse(xFunctionBlock.Attribute("Id").Value);

            var declaration = xFunctionBlock.Element("Declaration").Value;

            functionBlock.Declaration = declaration;
            functionBlock.Implementation = xFunctionBlock.Element("Implementation").Element("ST").Value;

            string lineEndingOfFile = declaration.Contains("\r\n") ? "\r\n" : "\n";

            string[] lines = declaration.Split(
                new[] { lineEndingOfFile },
                StringSplitOptions.None
            );

            foreach (string line in lines)
            {
                if (line.Trim().Length == 0) { continue; }

                if (line.Contains("FUNCTION_BLOCK"))
                {
                    string[] splitDefinition = Regex
                        .Split(line, @",|\s+", RegexOptions.IgnorePatternWhitespace)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();


                    bool implements = false;
                    bool extends = false;

                    foreach (string part in splitDefinition)
                    {
                        bool extendsStarts = (part.ToUpperInvariant() == "EXTENDS");
                        if (implements && !extendsStarts)
                        {
                            functionBlock.Implements.Add(part);
                        }

                        bool implementsStarts = (part.ToUpperInvariant() == "IMPLEMENTS");
                        if (extends && !implementsStarts)
                        {
                            functionBlock.Extends = part;
                        }

                        if (implementsStarts)
                        {
                            implements = true;
                            extends = false;
                        }
                        else if (extendsStarts)
                        {
                            extends = true;
                            implements = false;
                        }
                        else if (
                            part.ToUpperInvariant() == "ABSTRACT"
                            || part.ToUpperInvariant() == "FINAL"
                            || part.ToUpperInvariant() == "INTERNAL"
                            || part.ToUpperInvariant() == "PUBLIC"
                        )
                        {
                            functionBlock.AccessModifiers.Add(part);
                        }
                    }

                    break; // do not continue if functionblock declaration is found
                }

            }

            foreach (XElement xMethod in xFunctionBlock.Elements("Method"))
            {
                var method = Method_POU.Parse(xMethod);
                functionBlock.Methods.Add(method.Name, method);
            }

            return functionBlock;
        }
    }
}
