using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Log
{
    /// <summary>
    /// Aggregates multiple logs.
    /// </summary>
    public class LogAggregate
    {
        private List<ILog> _logs = new List<ILog>();
        private readonly object _sync = new object();

        public LogAggregate()
        {
        }

        /// <summary>
        /// Add a logger.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public LogAggregate AddLogger(ILog log)
        {
            lock (_sync)
            {
                _logs.Add(log);
            }
            return this;
        }

        /// <summary>
        /// Create ILog based on all added loggers.
        /// </summary>
        /// <returns></returns>
        public ILog CreateLogger()
        {
            return new Logger(_logs);
        }

        /// <summary>
        /// Sends log messages to all specified loggers.
        /// </summary>
        private class Logger : ILog
        {
            private List<ILog> _logs;

            public Logger(IEnumerable<ILog> logs)
            {
                this._logs = new List<ILog>(logs);
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
        }
    }
}
