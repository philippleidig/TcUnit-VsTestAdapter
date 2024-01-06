using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class StructuredTextImplementation 
    {
        private XCData _implementation;
        public int LineNumber { get; private set; }

        public StructuredTextImplementation(XElement implementation)
        {
            _implementation = implementation.DescendantNodes()
                                            .OfType<XCData>()
                                            .FirstOrDefault();
            var xmlLineInfo = implementation as IXmlLineInfo;

            if (!xmlLineInfo.HasLineInfo()) {
                throw new ArgumentOutOfRangeException("XML does not contain line information.");
            }

            LineNumber = xmlLineInfo.LineNumber;
        }

        public override string ToString()
        {
            return _implementation.Value;
        }
    }
}
