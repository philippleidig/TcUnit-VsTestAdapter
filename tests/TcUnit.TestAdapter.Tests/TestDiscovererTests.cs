using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Discovery;

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class TestDiscovererTests : TestContainer
    {
        [TestMethod]
        public void TestDiscoverTests()
        {
            // Arrange
            var mockLogger = Mock.Of<IMessageLogger>();
            var mockDiscoveryContext = Mock.Of<IDiscoveryContext>();

            var testCaseDiscoverySink = new TestCaseDiscoverySink();
            var testDiscoverer = new TestDiscoverer();

            var testSources = new List<string>() { @"PlcTestProject\PlcTestProject.tsproj" };

            // Act
            testDiscoverer.DiscoverTests(testSources, mockDiscoveryContext, mockLogger, testCaseDiscoverySink);

            // Assert
            Assert.IsTrue(testCaseDiscoverySink.TestCases.Count == 17); 
        }
    }

    internal class TestCaseDiscoverySink : ITestCaseDiscoverySink
    {
        public List<TestCase> TestCases = new List<TestCase>();
        public void SendTestCase(TestCase discoveredTest)
        {
            TestCases.Add(discoveredTest);
        }
    }
}

