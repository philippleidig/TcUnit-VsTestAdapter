using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Castle.Core.Logging;
using Moq;

namespace TcUnit.TestAdapter.Execution
{

    [TestClass]
    public class TestRunnerTests
    {
        [TestMethod]
        public void TestDiscoverTests()
        {
            var filePath = @"PlcTestProject\PlcTestProject.tsproj";

            var project = TwinCATXAEProject.Load(filePath);

            var logger = Mock.Of<IMessageLogger>();

            var testRunner = new TestRunner();

            var testCases = testRunner.DiscoverTests(project, logger);

            Assert.IsTrue(testCases.Count() == 2);
        }
    }
}
