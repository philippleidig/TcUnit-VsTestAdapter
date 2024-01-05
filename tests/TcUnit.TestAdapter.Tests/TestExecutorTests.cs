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

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class TestExecutorTests
    {
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
