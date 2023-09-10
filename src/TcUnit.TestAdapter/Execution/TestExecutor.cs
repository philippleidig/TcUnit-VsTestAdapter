using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Discovery;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter
{
    [ExtensionUri(TestAdapter.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {

        private readonly ITestRunner testRunner;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly object obj = new object();

        private bool _isCancelled;

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
            lock (obj)
            {
                cancellationTokenSource.Cancel();
                _isCancelled = true;
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            ValidateArg.NotNull(sources, nameof(sources));
            ValidateArg.NotNull(runContext, nameof(runContext));
            ValidateArg.NotNull(frameworkHandle, nameof(frameworkHandle));


            if (sources.Count() > 1)
            {
                throw new NotSupportedException("Only one TwinCAT XAE project (*.tsproj) is supported.");
            }

            try
            {
                var settings = runContext.RunSettings?.GetTestSettings(TestAdapter.RunSettingsName);
                var testCaseFilter = new TestCaseFilter(runContext);

                var project = TwinCATXAEProject.Load(sources.First());

                var tests = testRunner.DiscoverTests(project, frameworkHandle)
                                            .Where(t => testCaseFilter.MatchTestCase(t));

                if (!tests.Any())
                {
                    throw new ArgumentOutOfRangeException("Source does not contain any test case.");
                }

                var testRun = testRunner.RunTests(project, tests, settings, frameworkHandle as IMessageLogger);

                PrintRunConditions(frameworkHandle, testRun.Conditions);

                foreach(var testResult in testRun.Results)
                {
                    frameworkHandle.RecordResult(testResult);
                }
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }

        private void PrintRunConditions (IMessageLogger logger, TestRunConditions conditions)
        {
            logger.SendMessage(TestMessageLevel.Informational, "--------------------------------------------------------------");
            logger.SendMessage(TestMessageLevel.Informational, "Test Run Conditions:");
            logger.SendMessage(TestMessageLevel.Informational, "    Target AmsNetID: " + conditions.Target);
            logger.SendMessage(TestMessageLevel.Informational, "    TwinCAT Build: " + conditions.TwinCATVersion);
            logger.SendMessage(TestMessageLevel.Informational, "    Operating System: " + conditions.OperatingSystem);
            logger.SendMessage(TestMessageLevel.Informational, "    Duration: " + conditions.Duration.TotalSeconds.ToString() + "s");
            logger.SendMessage(TestMessageLevel.Informational, "    Configuration: " + conditions.BuildConfiguration);
            logger.SendMessage(TestMessageLevel.Informational, "--------------------------------------------------------------");
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var sources = tests.Select(test => test.Source).Distinct();
            RunTests(sources, runContext, frameworkHandle);
        }

    }
}
