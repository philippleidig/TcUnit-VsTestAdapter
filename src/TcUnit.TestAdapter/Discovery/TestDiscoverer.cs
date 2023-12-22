using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Execution;
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
            ValidateArg.NotNull(discoverySink, nameof(discoverySink));
            ValidateArg.NotNull(logger, nameof(logger));

            if(sources.Count() > 1)
                throw new NotSupportedException("Only one TwinCAT XAE project (*.tsproj) is supported.");

            try
            {
                var settings = discoveryContext.RunSettings?.GetTestSettings(TestAdapter.RunSettingsName);
                
                var testCaseFilter = new TestCaseFilter(discoveryContext, logger);

                var project = TwinCATXAEProject.Load(sources.First());

                var testCases = testRunner.DiscoverTests(project, logger);

                // currently can't filter test cases unless we have a run context
                if (discoveryContext is IRunContext)
                     testCases = testCases.Where(t => testCaseFilter.MatchTestCase(t));

                if (!testCases.Any())
                    throw new ArgumentOutOfRangeException("Source does not contain any test case.");

                foreach (var testCase in testCases)
                {
                    discoverySink.SendTestCase(testCase);
                }
            }
            catch (Exception ex)
            {
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }
    }
}
