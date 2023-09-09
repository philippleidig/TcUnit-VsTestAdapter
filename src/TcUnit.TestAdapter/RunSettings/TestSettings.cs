using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.RunSettings
{
    [XmlRoot(TestAdapter.RunSettingsName)]
    public class TestSettings : TestRunSettings
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(TestSettings));

        public TestSettings() : base(TestAdapter.RunSettingsName)
        {

        }

        [DefaultValue(TestAdapter.DefaultTargetRuntime)]
        public string Target { get; set; }

        [DefaultValue(TestAdapter.DefaultCleanUpAfterTestRun)]
        public bool CleanUpAfterTestRun { get; set; }

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
