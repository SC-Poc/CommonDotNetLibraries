using System;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log
{
    internal sealed class Log : ILog
    {
        private readonly ILogger _logger;

        public Log(ILogger logger)
        {
            _logger = logger;
        }

        void ILog.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        IDisposable ILog.BeginScope(string scopeMessage)
        {
            return _logger.BeginScope(scopeMessage);
        }

        #region Not implemented obsolete methods

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

        #endregion
    }
}
