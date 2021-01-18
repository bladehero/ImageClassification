using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace ImageClassification.API.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogWithName(this ILogger logger, LogLevel logLevel, Exception exception, [CallerMemberName] string name = "")
        {
            logger.Log(logLevel, exception, $"An error occured while executing: `{name}`");
        }

        public static void LogErrorWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogError(exception, $"An error occured while executing: `{name}`");
        }

        public static void LogTraceWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogTrace(exception, $"An error occured while executing: `{name}`");
        }

        public static void LogDebugWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogDebug(exception, $"An error occured while executing: `{name}`");
        }

        public static void LogInformationWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogInformation(exception, $"An error occured while executing: `{name}`");
        }

        public static void LogWarningWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogWarning(exception, $"An error occured while executing: `{name}`");
        }

        public static void LogCriticalWithName(this ILogger logger, Exception exception, [CallerMemberName] string name = "")
        {
            logger.LogCritical(exception, $"An error occured while executing: `{name}`");
        }
    }
}
