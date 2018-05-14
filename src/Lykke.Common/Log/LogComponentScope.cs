using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Log
{
    [Obsolete("Use new Lykke.Common.Log.ComponentLog")]
    internal class LogComponentScope : ILog
    {
        private readonly string _component;
        private readonly ILog _impl;

        public LogComponentScope(string component, ILog impl)
        {
            _component = component;
            _impl = impl;
        }

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
            return _impl.WriteInfoAsync($"{_component}:{component}", process, context, info, dateTime);
        }

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return _impl.WriteMonitorAsync($"{_component}:{component}", process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return _impl.WriteWarningAsync($"{_component}:{component}", process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return _impl.WriteWarningAsync($"{_component}:{component}", process, context, info, ex, dateTime);
        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return _impl.WriteErrorAsync($"{_component}:{component}", process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception,
            DateTime? dateTime = null)
        {
            return _impl.WriteFatalErrorAsync($"{_component}:{component}", process, context, exception, dateTime);
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return _impl.WriteInfoAsync(_component, process, context, info, dateTime);
        }

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return _impl.WriteMonitorAsync(_component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return _impl.WriteWarningAsync(_component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return _impl.WriteWarningAsync(process, context, info, ex, dateTime);
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return _impl.WriteErrorAsync(_component, process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return _impl.WriteFatalErrorAsync(_component, process, context, exception, dateTime);
        }
    }
}
