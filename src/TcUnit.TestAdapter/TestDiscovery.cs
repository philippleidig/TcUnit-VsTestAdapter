using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TcUnit.TestAdapter
{
    [FileExtension(".tsproj")]
    [DefaultExecutorUri(TestExecutor.ExecutorUriString)]
    public class TestDiscovery : ITestDiscoverer
    {

        // C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\TestPlatform\Extensions
        // C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform\Extensions
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            try
            {
                logger.SendMessage(TestMessageLevel.Informational, "Discover Tests");
                
                foreach (var source in sources)
                {
                    logger.SendMessage(TestMessageLevel.Informational, source);
                }

                GetTests(sources, discoverySink);

                logger.SendMessage(TestMessageLevel.Informational, "Tests discovered");

            }
            catch (Exception ex)
            {
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var tests = new List<TestCase>();

            foreach (var source in sources)
            {
                var test = new TestCase("Tc3_Motion.DriveGeneric_Test.ShouldReturnErrorCode", TestExecutor.ExecutorUri, source);
                test.LineNumber = 0;
                test.CodeFilePath = "DriveGeneric_Test.TcPOU";
                test.DisplayName = "Tc3_Motion.DriveGeneric_Test.ShouldReturnErrorCode";
             
                tests.Add(test);

                if (discoverySink != null)
                {
                    discoverySink.SendTestCase(test);
                }

                var test2 = new TestCase("Tc3_Motion.AxisGeneric_Test.ShouldReturnErrorCode", TestExecutor.ExecutorUri, source);
                test2.LineNumber = 0;
                test2.CodeFilePath = "AxisGeneric_Test.TcPOU";
                test2.DisplayName = "Tc3_Motion.AxisGeneric_Test.Should_ThrowInvalidData_When_AxisIdIsZero";

                tests.Add(test2);

                if (discoverySink != null)
                {
                    discoverySink.SendTestCase(test2);
                }
            }

            return tests;
        }
    }
}
