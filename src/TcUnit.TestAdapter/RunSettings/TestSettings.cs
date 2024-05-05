using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.RunSettings
{
    [XmlRoot(TestAdapter.RunSettingsName)]
    public class TestSettings : TestRunSettings
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(TestSettings));

        public TestSettings() 
            : base(TestAdapter.RunSettingsName)
        {
            Target = TestAdapter.DefaultTargetRuntime;
            CleanUpAfterTestRun = TestAdapter.DefaultCleanUpAfterTestRun;
            TimeoutSeconds = TestAdapter.DefaultTimeoutSeconds;
        }

        public string Target { get; set; } 
        public bool CleanUpAfterTestRun { get; set; }

        public double TimeoutSeconds { get; set; } = 10;

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
