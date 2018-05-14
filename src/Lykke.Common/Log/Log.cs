using System;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log
{
    public sealed class Log : ILog
    {
        private readonly ILogger _logger;

        public Log(ILogger logger)
        {
            _logger = logger;
        }

        void ILog.Write(LogLevel logLevel, EventId eventId, string message, object context, Exception exception, DateTime? moment, string appName, string appVersion, string envInfo)
        {
            _logger.Log(
                logLevel,
                eventId,
                new LogEntryParameters(appName, appVersion, envInfo, message, context, moment ?? DateTime.UtcNow),
                exception,
                (parameters, ex) => message);
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        IDisposable ILog.BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        Task ILog.WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteWarningAsync(string component, string process, string context, string info, Exception ex,
            DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteFatalErrorAsync(string component, string process, string context, Exception exception,
            DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteInfoAsync(string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteMonitorAsync(string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteWarningAsync(string process, string context, string info, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }

        Task ILog.WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
