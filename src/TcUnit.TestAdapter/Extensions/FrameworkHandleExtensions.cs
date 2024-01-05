﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;

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
    }
}
