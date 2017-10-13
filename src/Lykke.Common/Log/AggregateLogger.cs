using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Log
{
    /// <summary>
    /// Sends log messages to all specified loggers.
    /// </summary>
    public class AggregateLogger : 
        ILog,
        IStopable
    {
        private readonly List<ILog> _logs;

        public AggregateLogger(params ILog[] logs) :
            this((IEnumerable<ILog>)logs)
        {
        }

        public AggregateLogger(IEnumerable<ILog> logs)
        {
            _logs = new List<ILog>(logs);
        }
        
        public void AddLog(ILog log)
        {
            _logs.Add(log);
        }

        public bool RemoveLog(ILog log)
        {
            return _logs.Remove(log);
        }

        public void RemoveAllLogs()
        {
            _logs.Clear();
        }

        public void Stop()
        {
            foreach (var stopable in _logs.OfType<IStopable>())
            {
                stopable.Stop();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = default(DateTime?))
            => Task.WhenAll(_logs.Select(l => l.WriteInfoAsync(component, process, context, info, dateTime)));

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteMonitorAsync(component, process, context, info, dateTime)));

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = default(DateTime?))
            => Task.WhenAll(_logs.Select(l => l.WriteWarningAsync(component, process, context, info, dateTime)));

        public Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteWarningAsync(component, process, context, info, ex, dateTime)));

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = default(DateTime?))
            => Task.WhenAll(_logs.Select(l => l.WriteErrorAsync(component, process, context, exception, dateTime)));

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = default(DateTime?))
            => Task.WhenAll(_logs.Select(l => l.WriteFatalErrorAsync(component, process, context, exception, dateTime)));

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteInfoAsync(process, context, info, dateTime)));

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteMonitorAsync(process, context, info, dateTime)));

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteWarningAsync(process, context, info, dateTime)));

        public Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteWarningAsync(process, context, info, ex, dateTime)));

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteErrorAsync(process, context, exception, dateTime)));

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
            => Task.WhenAll(_logs.Select(l => l.WriteFatalErrorAsync(process, context, exception, dateTime)));
    }
}
