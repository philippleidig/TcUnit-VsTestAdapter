using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;
using System.Xml;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class TestSettingsTests
    {
        [TestMethod]
        public void TestDefaultValues ()
        {
            var settings = new TestSettings ();

            Assert.AreEqual(TestAdapter.DefaultTargetRuntime, settings.Target);
            Assert.AreEqual(TestAdapter.DefaultCleanUpAfterTestRun, settings.CleanUpAfterTestRun);
        }

        [TestMethod]
        public void TestDefaultValuesInRunContext()
        {
            var runSettings = Mock.Of<IRunSettings>();

            var settings = runSettings.GetTestSettings(TestAdapter.RunSettingsName);

            Assert.AreEqual(TestAdapter.DefaultTargetRuntime, settings.Target);
            Assert.AreEqual(TestAdapter.DefaultCleanUpAfterTestRun, settings.CleanUpAfterTestRun);
        }

        [TestMethod]
        public void TestBadXmlElements ()
        {
            string settingsXml =
              @"<?xml version=""1.0"" encoding=""utf-8""?>
                <RunSettings>
                     <TcUnit>
                       <BadElement>TestResults</BadElement>
                       <Target>192.168.0.1.1.1</Target>
                     </TcUnit>
                </RunSettings>";

            var runSettingsProvider = new RunSettingsProvider();

            using(XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
            {
                runSettingsProvider.Load(reader);
            }

            var settings = runSettingsProvider.Settings;

            Assert.AreEqual("192.168.0.1.1.1", settings.Target);
            Assert.AreEqual(TestAdapter.RunSettingsName, runSettingsProvider.Name);
        }

        [TestMethod]
        public void TestInvalidXmlValues()
        {
            string settingsXml =
              @"<?xml version=""1.0"" encoding=""utf-8""?>
                <RunSettings>
                     <TcUnit>
                       <Target>12345678</Target>
                     </TcUnit>
                </RunSettings>";

            var runSettingsProvider = new RunSettingsProvider();

            using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
            {
                runSettingsProvider.Load(reader);
            }

            var settings = runSettingsProvider.Settings;


        }
    }
}
