using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;
using System.Xml;
using TcUnit.TestAdapter.RunSettings;
using TwinCAT.Ads;

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
        public void TestInRunContext()
        {
            string settingsXml =
              @"<TcUnit>
                   <CleanUpAfterTestRun>false</CleanUpAfterTestRun>
                   <Target>192.168.0.1.1.1</Target>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
            {
                runSettingsProvider.Load(reader);
            }

            var runSettings = new Mock<IRunSettings>();
            runSettings.Setup(x => x.GetSettings(It.IsAny<string>()))
                              .Returns(runSettingsProvider);

            var settings = runSettings.Object.GetTestSettings(TestAdapter.RunSettingsName);

            Assert.AreEqual("192.168.0.1.1.1", settings.Target);
            Assert.AreEqual(false, settings.CleanUpAfterTestRun);
            Assert.AreEqual(TestAdapter.RunSettingsName, runSettingsProvider.Name);
        }

        [TestMethod]
        public void TestBadXmlElements ()
        {
            string settingsXml =
              @"<TcUnit>
                    <BadElement>TestResults</BadElement>
                    <Target>192.168.0.1.1.1</Target>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            Assert.ThrowsException<InvalidTestSettingsException>(() => {
                using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
                {
                    runSettingsProvider.Load(reader);
                }
            });
        }

        [TestMethod]
        public void TestInvalidXmlRootValue()
        {
            string settingsXml =
              @"<TcUnitInvalid>
                   <CleanUpAfterTestRun>false</CleanUpAfterTestRun>
                   <Target>192.168.0.1.1.1</Target>
                </TcUnitInvalid>";

            var runSettingsProvider = new RunSettingsProvider();

            Assert.ThrowsException<InvalidTestSettingsException>(() => {
                using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
                {
                    runSettingsProvider.Load(reader);
                }
            });
        }

        [TestMethod]
        public void TestInvalidXmlTargetValue()
        {
            string settingsXml =
              @"<TcUnit>
                    <Target>12345678</Target>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            Assert.ThrowsException<InvalidTestSettingsException>(() => {
                using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
                {
                    runSettingsProvider.Load(reader);
                }
            });
        }

        [TestMethod]
        public void TestInvalidXmlCleanUpValue()
        {
            string settingsXml =
              @"<TcUnit>
                    <CleanUpAfterTestRun>abcdefg</CleanUpAfterTestRun>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            Assert.ThrowsException<InvalidTestSettingsException>(() => {
                using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
                {
                    runSettingsProvider.Load(reader);
                }
            });
        }

        [TestMethod]
        public void TestDuplicateXmlEntry()
        {
            string settingsXml =
              @"<TcUnit>
                   <CleanUpAfterTestRun>false</CleanUpAfterTestRun>
                   <Target>192.168.0.1.1.1</Target>
                   <Target>192.168.0.1.1.1</Target>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            Assert.ThrowsException<InvalidTestSettingsException>(() => {
                using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
                {
                    runSettingsProvider.Load(reader);
                }
            });
        }

        [TestMethod]
        public void TestXmlElements()
        {
            string settingsXml =
              @"<TcUnit>
                   <CleanUpAfterTestRun>false</CleanUpAfterTestRun>
                   <Target>192.168.0.1.1.1</Target>
                </TcUnit>";

            var runSettingsProvider = new RunSettingsProvider();

            using (XmlReader reader = XmlReader.Create(new StringReader(settingsXml)))
            {
                runSettingsProvider.Load(reader);
            }

            var settings = runSettingsProvider.Settings;

            Assert.AreEqual("192.168.0.1.1.1", settings.Target);
            Assert.AreEqual(false, settings.CleanUpAfterTestRun);
            Assert.AreEqual(TestAdapter.RunSettingsName, runSettingsProvider.Name);
        }
    }
}
