using System;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Lykke.Common.Log
{
    public sealed class Log<TComponent> : ILog<TComponent>
    {
        private readonly ILog _log;

        public Log(ILogFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _log = factory.CreateLog(TypeNameHelper.GetTypeDisplayName(typeof(TComponent)));
        }

        void ILog.Write(LogLevel logLevel, EventId eventId, string message, object context, Exception exception, DateTime? moment, string appName, string appVersion, string envInfo)
        {
            _log.Write(
                logLevel,
                eventId,
                message,
                context,
                exception,
                moment,
                appName,
                appVersion,
                envInfo);
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            return _log.IsEnabled(logLevel);
        }

        IDisposable ILog.BeginScope<TState>(TState state)
        {
            return _log.BeginScope(state);
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
