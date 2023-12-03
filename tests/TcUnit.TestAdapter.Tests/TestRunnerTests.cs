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
using TcUnit.TestAdapter.RunSettings;

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

            List<string> testCaseNames = new List<string>()
            { "PRG_TESTS.fbTestSuite1Instance1.TestCase1A",
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1B",
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1C",
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1D",
              "PRG_TESTS.fbTestSuite2Instance1.TestCase2A",
              "PRG_TESTS.fbTestSuite2Instance1.TestCase2B",
              "PRG_TESTS.fbTestSuite2Instance1.TestCase2C",
              "PRG_TESTS.fbTestSuite2Instance2.TestCase2A",
              "PRG_TESTS.fbTestSuite2Instance2.TestCase2B",
              "PRG_TESTS.fbTestSuite2Instance2.TestCase2C",
            };

            Assert.IsTrue(testCases.Count() == testCaseNames.Count);

            foreach (var testCase in testCases)
            {
                Assert.IsTrue(testCaseNames.Contains(testCase.FullyQualifiedName));
            }
        }

        [TestMethod]
        public void RunTestsTests()
        {
            var filePath = @"PlcTestProject\PlcTestProject.tsproj";
            var project = TwinCATXAEProject.Load(filePath);
            var logger = Mock.Of<IMessageLogger>();

            var testRunner = new TestRunner();
            var tests = testRunner.DiscoverTests(project, logger);

            var settings = new TestSettings();
            settings.Target = "172.18.232.132.1.1";
            settings.CleanUpAfterTestRun = true;

            testRunner.RunTests(project, tests, settings, logger);
        }
    }
}
