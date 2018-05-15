using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Common.Log
{
    // TODO: Since only ILogFactory should be injected in the services, this class should be made internal and sealed when legacy logging system will be removed
    // TODO: Should be moved to the Lykke.Common.Log namespace, when legacy loggin system will be removed

    /// <summary>
    /// Log that do nothing. Used by the <see cref="EmptyLogFactory"/>
    /// </summary>
    [PublicAPI]
    public class EmptyLog : ILog
    {
        public static EmptyLog Instance { get; } = new EmptyLog();

        void ILog.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        IDisposable ILog.BeginScope(string scopeMessage)
        {
            return NullScope.Instance;
        }

        #region Obsolete methods

        public Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, Exception ex,
            DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
