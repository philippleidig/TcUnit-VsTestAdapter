using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Discovery
{
    [FileExtension(TestAdapter.PlcProjFileExtension)]
    [FileExtension(TestAdapter.TsProjFileExtension)]
    [DefaultExecutorUri(TestAdapter.ExecutorUriString)]
    public class TestDiscovery : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            ValidateArg.NotNull(sources, nameof(sources));
            ValidateArg.NotNull(discoverySink, nameof(discoverySink));
            ValidateArg.NotNull(logger, nameof(logger));

            try
            {
                var settingsProvider = discoveryContext.RunSettings.GetSettings(TestAdapter.RunSettingsName) as RunSettingsProvider;
                var settings = settingsProvider != null ? settingsProvider.Settings : new TestSettings();

                var testRunner = new TestRunner();
                var testCases = testRunner.DiscoverTests(sources, settings);

                foreach (var testCase in testCases)
                {
                    if (discoverySink != null)
                    {
                        discoverySink.SendTestCase(testCase);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }
    }
}
