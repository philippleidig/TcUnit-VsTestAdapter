using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;

namespace TcUnit.TestAdapter
{
    [ExtensionUri(TestExecutor.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        public const string ExecutorUriString = "executor://TcUnitTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        private bool _isCancelled;
        public void Cancel()
        {
            _isCancelled = true;
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            try
            {
                foreach(TestCase test in tests)
                {
                    if(_isCancelled)
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
            //result.Outcome = TestOutcome.Failed;
            //result.Outcome = TestOutcome.Skipped;

            // TODO - run tcunit tests
            // var testRunner = new TcUnitTestRunner();

            //result.ErrorStackTrace = "error stack trace";
            //result.ErrorMessage = "error message";

            return result;
        }
    }
}
