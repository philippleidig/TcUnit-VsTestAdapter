using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using TcUnit.TestAdapter.Tests.Mocks;

namespace TcUnit.TestAdapter.Discovery
{
    [TestClass]
    public class TestDiscovererTests
    {
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
            Assert.IsTrue(testCaseDiscoverySink.TestCases.Count == 17); 
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
            Assert.IsTrue(testCaseDiscoverySink.TestCases.Count == 34);
        }
    }


}

