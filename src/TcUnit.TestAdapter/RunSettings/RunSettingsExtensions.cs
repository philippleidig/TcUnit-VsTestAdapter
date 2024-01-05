using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace TcUnit.TestAdapter.RunSettings
{
    public static class RunSettingsExtensions
    {
        public static RunSettingsProvider GetTestSettingsProvider(this IRunSettings runSettings, string name)
        {
            return runSettings.GetSettings(name) as RunSettingsProvider;
        }

        public static TestSettings GetTestSettings(this IRunSettings runSettings, string name)
        {
            var provider = runSettings.GetTestSettingsProvider(name);
            if (provider == null)
            {
                return new TestSettings();
            }

            return provider.Settings as TestSettings;
        }
    }
}
