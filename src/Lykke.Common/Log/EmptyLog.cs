using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Log
{
    [Obsolete("Use new Lykke.Common.Log.Loggers.EmptyLogger")]
    public class EmptyLog : ILog
    {
        public static EmptyLog Instance { get; } = new EmptyLog();

        public void Write(LogLevel logLevel, EventId eventId, string message, object context, Exception exception, DateTime? moment,
            string appName, string appVersion, string envInfo)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

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
    }
}
