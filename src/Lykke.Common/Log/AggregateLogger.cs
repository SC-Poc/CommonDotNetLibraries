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

        public AggregateLogger() :
            this(Enumerable.Empty<ILog>())
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

        public async Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = default(DateTime?))
        {
            foreach (var log in _logs)
            {
                await log.WriteErrorAsync(component, process, context, exception, dateTime);
            }
        }

        public async Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = default(DateTime?))
        {
            foreach (var log in _logs)
            {
                await log.WriteFatalErrorAsync(component, process, context, exception, dateTime);
            }
        }

        public async Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = default(DateTime?))
        {
            foreach (var log in _logs)
            {
                await log.WriteInfoAsync(component, process, context, info, dateTime);
            }
        }

        public async Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = default(DateTime?))
        {
            foreach (var log in _logs)
            {
                await log.WriteWarningAsync(component, process, context, info, dateTime);
            }
        }

        public void Stop()
        {
            foreach (var stopable in _logs.OfType<IStopable>())
            {
                stopable.Stop();
            }
        }
    }
}