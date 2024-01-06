using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using TcUnit.TestAdapter.Execution;

namespace TcUnit.TestAdapter.Extensions
{
    public static class FrameworkHandleExtensions
    {
        public static void LogInformation(this IFrameworkHandle frameworkHandle, string message)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, message);
        }

        public static void LogWarning(this IFrameworkHandle frameworkHandle, string message)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Warning, message);
        }

        public static void LogError(this IFrameworkHandle frameworkHandle, string message)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Error, message);
        }

        public static void LogError(this IFrameworkHandle frameworkHandle, string message, Exception ex)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Error, string.Format("{0} \nReason: {1}", message, ex.Message));
        }

        public static void PrintTestRunConditions(this IFrameworkHandle frameworkHandle, TestRunContext context)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "--------------------------------------------------------------");
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test Run Conditions:");
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "    Target AmsNetID: " + context.Target.ToString());
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "    TwinCAT Build: " + context.TwinCATVersion.ToString());
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "    Operating System: " + context.OperatingSystem);
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "    Duration: " + context.Duration.TotalSeconds.ToString() + "s");
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "    Configuration: " + context.BuildConfiguration);
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "--------------------------------------------------------------");
        }
    }
}
