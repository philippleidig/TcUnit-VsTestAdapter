using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.RunSettings
{
    public class TestSettings : TestRunSettings
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(TestSettings));

        public TestSettings() : base(TestAdapter.RunSettingsName)
        {

        }

        public string Target;
        public bool CleanUpAfterTestRun;

        public override XmlElement ToXml()
        {
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, this);
            var xml = stringWriter.ToString();
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document.DocumentElement;
        }
    }
}
