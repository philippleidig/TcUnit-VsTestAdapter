using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Discovery;
using TcUnit.TestAdapter.RunSettings;
using TcUnit.TestAdapter.Tests.Mocks;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace TcUnit.TestAdapter.Execution
{
    [TestClass]
    public class TestExecutorTests
    {
        [TestMethod]
        public void TestExecuteTestsEmptySources()
        {
            Assert.ThrowsException<ArgumentException>(() => {
                new TestExecutor().RunTests(new List<string>() , Mock.Of<IRunContext>(), Mock.Of<IFrameworkHandle>());
            });
        }

        [TestMethod]
        public void TestExecuteTestsNullSources()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                IEnumerable<string> sources = null;
                new TestExecutor().RunTests(sources, Mock.Of<IRunContext>(), Mock.Of<IFrameworkHandle>());
            });
        }

        [TestMethod]
        public void TestExecuteTestsNullRunContext()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestExecutor().RunTests(new List<string>() { "" }, null, Mock.Of<IFrameworkHandle>());
            });
        }

        [TestMethod]
        public void TestExecuteTestsNullFrameworkHandle()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                new TestExecutor().RunTests(new List<string>() { "" }, Mock.Of<IRunContext>(), null);
            });
        }

        [TestMethod]
        public void TestExecuteTestsThrowsPlcException()
        {
            // Arrange
            var frameworkHandle = new FrameworkHandleMock();
        
            var runSettings = new Mock<IRunSettings>();
            runSettings.Setup(x => x.GetSettings(It.IsAny<string>()))
                              .Returns(() => null);
        
            var runContext = new Mock<IRunContext>();
            runContext.Setup(x => x.RunSettings)
                  .Returns(runSettings.Object);
        
            var testExecutor = new TestExecutor();
        
            var testSources = new List<string>() { @"ThrowsPlcException\ThrowsPlcException.tsproj" };
        
            // Act
            testExecutor.RunTests(testSources, runContext.Object, frameworkHandle);
        
            // Assert
            Assert.AreEqual(0, frameworkHandle.TestResults.Count);
        }

        [TestMethod]
        public void TestExecuteTestsInSingleSource()
        {
            // Arrange
            var frameworkHandle = new FrameworkHandleMock();

            var runSettings = new Mock<IRunSettings>();
            runSettings.Setup(x => x.GetSettings(It.IsAny<string>()))
                              .Returns(() => null);

            var runContext = new Mock<IRunContext>();
            runContext.Setup(x => x.RunSettings)
                  .Returns(runSettings.Object);

            var testExecutor = new TestExecutor();

            var testSources = new List<string>() { @"PlcTestProject\PlcTestProject.tsproj" };

            // Act
            testExecutor.RunTests(testSources, runContext.Object, frameworkHandle);

            // Assert
            Assert.AreEqual(17, frameworkHandle.TestResults.Count);
        }

        [TestMethod]
        public void TestExecuteTestsInMultipleSources()
        {
            // Arrange
            var frameworkHandle = new FrameworkHandleMock();

            var runSettings = new Mock<IRunSettings>();
            runSettings.Setup(x => x.GetSettings(It.IsAny<string>()))
                              .Returns(() => null);

            var runContext = new Mock<IRunContext>();
            runContext.Setup(x => x.RunSettings)
                  .Returns(runSettings.Object);

            var testExecutor = new TestExecutor();

            var testSources = new List<string>() {
                                @"PlcTestProject\PlcTestProject.tsproj",
                                @"PlcTestProject\PlcTestProject.tsproj" 
                            };

            // Act
            testExecutor.RunTests(testSources, runContext.Object, frameworkHandle);

            // Assert
            Assert.AreEqual(34, frameworkHandle.TestResults.Count);
        }
    }
}
