using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Discovery;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.Extensions;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter
{
    [ExtensionUri(TestAdapter.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private readonly ITestRunner testRunner;
        private TestRunCancellationToken _cancellationToken;

        public TestExecutor ()
        {
            this.testRunner = new TestRunner();
        }

        public TestExecutor(ITestRunner testRunner)
        {
            this.testRunner = testRunner;
        }

        public void Cancel()
        {
            _cancellationToken?.Cancel();
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            ValidateArg.NotNullOrEmpty(sources, nameof(sources));
            ValidateArg.NotNull(runContext, nameof(runContext));
            ValidateArg.NotNull(frameworkHandle, nameof(frameworkHandle));

            try
            {
                var settings = runContext.RunSettings?.GetTestSettings(TestAdapter.RunSettingsName);
                var testCaseFilter = new TestCaseFilter(runContext, frameworkHandle);

                _cancellationToken = new TestRunCancellationToken();

                foreach (var source in sources)
                {
                    try
                    {
                        var project = TwinCATXAEProject.Load(source);
                        var tests = testRunner.DiscoverTests(project, frameworkHandle)
                                                  .Where(t => testCaseFilter.MatchTestCase(t));

                        if (!tests.Any())
                        {
                            frameworkHandle.LogInformation("Source does not contain any test case.");
                        }
                        else
                        {
                            frameworkHandle.LogInformation( string.Format("Found {0} test cases in source {1}", tests.Count(), source));
                        }

                        var testRun = testRunner.RunTests(project, tests, settings, frameworkHandle);

                        PrintRunConditions(frameworkHandle, source, testRun.Context);

                        foreach (var testResult in testRun.Results)
                        {
                            frameworkHandle.RecordResult(testResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        frameworkHandle.LogError(string.Format("Failed to execute test run for source {0}.", source), ex);
                    }
                }

                _cancellationToken = null;
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var sources = tests.Select(test => test.Source).Distinct();
            RunTests(sources, runContext, frameworkHandle);
        }
        private void PrintRunConditions(IMessageLogger logger, string source, TestRunContext context)
        {
            logger.LogInformation( "--------------------------------------------------------------");
            logger.LogInformation( "Test Run Conditions:");
            logger.LogInformation( "    Source: " + source);
            logger.LogInformation( "    Target AmsNetID: " + context.Target);
            logger.LogInformation( "    TwinCAT Build: " + context.TwinCATVersion);
            logger.LogInformation( "    Operating System: " + context.OperatingSystem);
            logger.LogInformation( "    Duration: " + context.Duration.TotalSeconds.ToString() + "s");
            logger.LogInformation( "    Configuration: " + context.BuildConfiguration);
            logger.LogInformation( "--------------------------------------------------------------");
        }
    }
}
