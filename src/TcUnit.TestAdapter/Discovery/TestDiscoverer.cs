using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.Extensions;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Discovery
{
    //[FileExtension(TestAdapter.PlcProjFileExtension)]
    [FileExtension(TestAdapter.TsProjFileExtension)]
    [DefaultExecutorUri(TestAdapter.ExecutorUriString)]
    public class TestDiscoverer : ITestDiscoverer
    {
        private readonly ITestRunner testRunner;

        public TestDiscoverer()
        {
            this.testRunner = new TestRunner();
        }

        public TestDiscoverer(ITestRunner testRunner)
        {
            this.testRunner = testRunner;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            ValidateArg.NotNullOrEmpty(sources, nameof(sources));
            ValidateArg.NotNull(discoveryContext, nameof(discoveryContext));
            ValidateArg.NotNull(discoverySink, nameof(discoverySink));
            ValidateArg.NotNull(logger, nameof(logger));

            try
            {
                var settings = discoveryContext.RunSettings?.GetTestSettings(TestAdapter.RunSettingsName);      
                var testCaseFilter = new TestCaseFilter(discoveryContext, logger);
                var tests = new List<TestCase>();

                foreach (var source in sources)
                {
                    var project = TwinCATXAEProject.Load(source);
                    var testCases = testRunner.DiscoverTests(project, logger)
                                              .Where(t => testCaseFilter.MatchTestCase(t));

                    if (!testCases.Any())
                        logger.LogInformation("Source does not contain any test case.");
                    else
                        logger.LogInformation(string.Format("Found {0} test cases in source {1}", testCases.Count(), source));

                    tests.AddRange(testCases);
                }

                if (!tests.Any())
                {
                    logger.LogWarning("Sources do not contain any test case.");
                    return;
                }
                    
                foreach (var testCase in tests)
                {
                    discoverySink.SendTestCase(testCase);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
