using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using TcUnit.TestAdapter.Tests.Mocks;
using System.Xml;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Discovery
{
    [TestClass]
    public class TestDiscovererTests
    {
        [TestMethod]
        public void TestDiscoverTestsEmptySources()
        {
            Assert.ThrowsException<ArgumentException>(() => {
                new TestDiscoverer().DiscoverTests(new List<string>(), Mock.Of<IDiscoveryContext>(), Mock.Of<IMessageLogger>(), new TestCaseDiscoverySinkMock());
            });
        }

        [TestMethod]
        public void TestDiscoverTestsNullSources()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestDiscoverer().DiscoverTests(null, Mock.Of<IDiscoveryContext>(), Mock.Of<IMessageLogger>(), new TestCaseDiscoverySinkMock());
            });
        }

        [TestMethod]
        public void TestDiscoverTestsNullDiscoveryContext()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestDiscoverer().DiscoverTests(new List<string>() { "" }, null, Mock.Of<IMessageLogger>(), new TestCaseDiscoverySinkMock());
            });
        }

        [TestMethod]
        public void TestDiscoverTestsNullMessageLogger()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestDiscoverer().DiscoverTests(new List<string>() { "" }, Mock.Of<IDiscoveryContext>(), null, new TestCaseDiscoverySinkMock());
            });
        }

        [TestMethod]
        public void TestDiscoverTestsNullDiscoverySink()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestDiscoverer().DiscoverTests(new List<string>() { "" }, Mock.Of<IDiscoveryContext>(), Mock.Of<IMessageLogger>(), null);
            });
        }

        [TestMethod]
        public void TestDiscoverTestsInSingleSource()
        {
            // Arrange
            var mockLogger = Mock.Of<IMessageLogger>();
            var mockDiscoveryContext = Mock.Of<IDiscoveryContext>();

            var testCaseDiscoverySink = new TestCaseDiscoverySinkMock();
            var testDiscoverer = new TestDiscoverer();

            var testSources = new List<string>() { @"PlcTestProject\PlcTestProject.tsproj" };

            // Act
            testDiscoverer.DiscoverTests(testSources, mockDiscoveryContext, mockLogger, testCaseDiscoverySink);

            // Assert
            Assert.AreEqual(17, testCaseDiscoverySink.TestCases.Count); 
        }

        [TestMethod]
        public void TestDiscoverTestsInMultipleSources()
        {
            // Arrange
            var mockLogger = Mock.Of<IMessageLogger>();
            var mockDiscoveryContext = Mock.Of<IDiscoveryContext>();

            var testCaseDiscoverySink = new TestCaseDiscoverySinkMock();
            var testDiscoverer = new TestDiscoverer();

            var testSources = new List<string>() {
                @"PlcTestProject\PlcTestProject.tsproj",
                @"PlcTestProject\PlcTestProject.tsproj" 
            };

            // Act
            testDiscoverer.DiscoverTests(testSources, mockDiscoveryContext, mockLogger, testCaseDiscoverySink);

            // Assert
            Assert.AreEqual(34,testCaseDiscoverySink.TestCases.Count);
        }
    }


}

