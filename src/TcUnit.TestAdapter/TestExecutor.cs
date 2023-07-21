using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcUnit.Core;

namespace TcUnit.TestAdapter
{
    [ExtensionUri(TestExecutor.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private readonly ITestRunner testRunner;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly object obj = new object();

        public const string ExecutorUriString = "executor://TcUnitTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        public const string FileExtension = ".tsproj";

        private bool _isCancelled;
        public void Cancel()
        {
            lock (obj)
            {
                cancellationTokenSource.Cancel();
                _isCancelled = true;
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //var sources = tests.Select(test => test.Source).Distinct();
            //RunTests(sources, runContext, frameworkHandle);

            try
            {
                foreach (TestCase test in tests)
                {
                    if (_isCancelled)
                    {
                        break;
                    }

                    frameworkHandle.RecordStart(test);



                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Starting external test for " + test.DisplayName);
                    var testOutcome = RunExternalTest(test, runContext, frameworkHandle);
                    frameworkHandle.RecordResult(testOutcome);
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test result: " + testOutcome.Outcome);
                }
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            try
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Run Unit Tests");

                IEnumerable<TestCase> tests = TestDiscovery.GetTests(sources, null);

                foreach (var test in tests)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, test.DisplayName);
                }

                RunTests(tests, runContext, frameworkHandle);

               // testRunner.RunTests(cancellationTokenSource.Token)
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, ex.Message);
            }

        }

        private TestResult RunExternalTest (TestCase test, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var result = new TestResult(test);

            result.Outcome = TestOutcome.Passed;

            result.Duration = TimeSpan.FromMilliseconds(10);

            // TODO - run tcunit tests

            // Use RPC for single test execution? 
            // how to set XAR in run? use selected target system?
            // use TwinCAT UserMode runtime?

            result.ErrorStackTrace = "errorstacktrace";
            result.ErrorMessage = "error message";

            return result;
        }
    }
}
