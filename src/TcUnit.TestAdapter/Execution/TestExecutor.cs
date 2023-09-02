using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter
{
    [ExtensionUri(TestAdapter.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {

        private readonly TestRunner testRunner = new TestRunner();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly object obj = new object();

        private bool _isCancelled;
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

            try
            {
                var settingsProvider = runContext.RunSettings.GetSettings(TestAdapter.RunSettingsName) as RunSettingsProvider;
                var settings = settingsProvider != null ? settingsProvider.Settings : new TestSettings();

                var tests = testRunner.DiscoverTests(sources, settings);

               // testRunner.RunTests(settings.Target, true);
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

    }
}
