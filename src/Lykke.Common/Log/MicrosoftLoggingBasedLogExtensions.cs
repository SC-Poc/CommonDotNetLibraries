using System;
using Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log
{
    public static class MicrosoftLoggingBasedLogExtensions
    {
        public static void Trace(this ILog log, string message)
        {
            log.Write(LogLevel.Trace, message);
        }

        public static void Debug(this ILog log, string message)
        {
            log.Write(LogLevel.Debug, message);
        }

        public static void Info(this ILog log, string message)
        {
            log.Write(LogLevel.Information, message);
        }

        public static void Info(this ILog log, EventId eventId, string message)
        {
            log.Write(LogLevel.Information, message, eventId: eventId);
        }

        public static void Info(this ILog log, EventId eventId, string message, object context)
        {
            log.Write(LogLevel.Information, message, context: context, eventId: eventId);
        }

        public static void Warning(this ILog log, string message)
        {
            log.Write(LogLevel.Warning, message);
        }

        public static void Error(this ILog log, string message)
        {
            log.Write(LogLevel.Error, message);
        }

        public static void Critical(this ILog log, string message)
        {
            log.Write(LogLevel.Critical, message);
        }

        public static void Critical(this ILog log, string message, Exception exception)
        {
            log.Write(LogLevel.Critical, message, exception: exception);
        }

        private static void Write(this ILog log, LogLevel logLevel, string message, object context = null, EventId? eventId = null, Exception exception = null, DateTime? moment = null)
        {
            log.Write(
                logLevel,
                eventId ?? 0,
                message,
                context,
                exception,
                moment,
                AppEnvironment.Name,
                AppEnvironment.Version,
                AppEnvironment.EnvInfo);
        }
    }
}
