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
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

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
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1E with a space",
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1F Ignore Case",
              "PRG_TESTS.fbTestSuite1Instance1.TestCase1G 1*/_-1^&\"@!()",
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
            // currently this requires the target boot folder to already be empty and the target in config mode
            var filePath = @"PlcTestProject\PlcTestProject.tsproj";
            var project = TwinCATXAEProject.Load(filePath);
            var logger = Mock.Of<IMessageLogger>();


            var settings = new TestSettings();
            settings.Target = "127.0.0.1.1.1";  //"192.168.4.1.1.1";
            settings.CleanUpAfterTestRun = true;

            // attempt to clean up the target boot folder
            var systemService = new SystemService(settings.Target);
            var targetInfo = systemService.GetDeviceInfo();
            systemService.SwitchRuntimeState(TwinCAT.Ads.AdsState.Reconfig);
            systemService.CleanUpBootDirectory(targetInfo.ImageOsName);
            systemService.Disconnect();
            
            var testRunner = new TestRunner();
            var tests = testRunner.DiscoverTests(project, logger);

            var testRun = testRunner.RunTests(project, tests, settings, logger);

            Assert.IsTrue(testRun.Results.Count() == 13);
        }
    }
}
