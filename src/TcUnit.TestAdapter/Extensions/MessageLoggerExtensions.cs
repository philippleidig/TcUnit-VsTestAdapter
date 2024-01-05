using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;

namespace TcUnit.TestAdapter.Extensions
{
    public static class MessageLoggerExtensions
    {
        public static void LogInformation(this IMessageLogger logger, string message)
        {
            logger.SendMessage(TestMessageLevel.Informational, message);
        }

        public static void LogWarning(this IMessageLogger logger, string message)
        {
            logger.SendMessage(TestMessageLevel.Warning, message);
        }

        public static void LogError(this IMessageLogger logger, string message)
        {
            logger.SendMessage(TestMessageLevel.Error, message);
        }

        public static void LogError(this IMessageLogger logger, string message, Exception ex)
        {
            logger.SendMessage(TestMessageLevel.Error, string.Format("{0} \nReason: {1}", message, ex.Message));
        }
    }
}
