using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.ComponentModel.Composition;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.RunSettings
{
    [SettingsName(TestAdapter.RunSettingsName)]
    [Export(typeof(ISettingsProvider))]
    public class RunSettingsProvider : ISettingsProvider
    {
        protected readonly XmlSerializer serializer;

        public TestSettings Settings { get; private set; }

        public string Name { get; private set; }

        public RunSettingsProvider()
        {
            Name = TestAdapter.RunSettingsName;
            Settings = new TestSettings();
            serializer = new XmlSerializer(typeof(TestSettings));
        }
        public void Load(XmlReader reader)
        {
            ValidateArg.NotNull(reader, "reader");

            if (reader.Read() && reader.Name.Equals(Name))
            {
                var runSettings = XDocument.Load(reader);
                var rootElement = runSettings.Element(TestAdapter.RunSettingsName);

                Settings.Target = rootElement.Element("Target")?.Value ?? TestAdapter.DefaultTargetRuntime;

                var cleanUpAfterRun = rootElement.Element("CleanUpAfterTestRun")?.Value;

                if (!string.IsNullOrEmpty(cleanUpAfterRun))
                Settings.CleanUpAfterTestRun = bool.Parse(cleanUpAfterRun);
            }
        }
    }
}
