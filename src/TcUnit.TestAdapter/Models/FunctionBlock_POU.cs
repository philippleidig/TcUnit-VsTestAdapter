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
        public Dictionary<string, Method_POU> Methods { get; private set; } = new Dictionary<string, Method_POU>();
        public string Extends { get; private set; }
        public List<string> AccessModifiers { get; private set; } = new List<string>();
        public List<string> Attributes { get; private set; } = new List<string>();
        public List<string> Implements { get; private set; } = new List<string>();

        private FunctionBlock_POU(string filepath)
            : this(XDocument.Load(filepath, LoadOptions.SetLineInfo))
        {
            var file = Path.GetFileName(filepath);

            Name = file.Replace(".TcPOU", "");
            FilePath = filepath;
        }

        private FunctionBlock_POU(XDocument document)
        {
            XElement xFunctionBlock = document.Element("TcPlcObject")
                                              .Elements("POU")
                                              .First<XElement>();

            Id = Guid.Parse(xFunctionBlock.Attribute("Id").Value);

            var implementation = xFunctionBlock.Element("Implementation").Element("ST");

            if (implementation != null)
            {
                Implementation = new StructuredTextImplementation(implementation);
            }

            var declaration = xFunctionBlock.Element("Declaration").Value;
            Declaration = declaration;

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
                            Implements.Add(part);
                        }

                        bool implementsStarts = (part.ToUpperInvariant() == "IMPLEMENTS");
                        if (extends && !implementsStarts)
                        {
                            Extends = part;
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
                            AccessModifiers.Add(part);
                        }
                    }

                    break; // do not continue if functionblock declaration is found
                }

            }

            foreach (XElement xMethod in xFunctionBlock.Elements("Method"))
            {
                try
                {
                    var method = Method_POU.Parse(xMethod);
                    Methods.Add(method.Name, method);
                }
                catch (NotSupportedException ex)
                {
                    // skip method due to implementation language
                }
            }
        }

        public static FunctionBlock_POU Parse(XDocument document)
        {
            return new FunctionBlock_POU(document);
        }

        public static FunctionBlock_POU Load(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            if (!filepath.Contains(".TcPOU"))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new FunctionBlock_POU(filepath);
        }
    }
}
