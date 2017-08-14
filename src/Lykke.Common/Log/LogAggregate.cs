using System.Collections.Generic;

namespace Common.Log
{
    /// <summary>
    /// Builder for <see cref="AggregateLogger"/>.
    /// </summary>
    public class LogAggregate
    {
        private readonly List<ILog> _logs = new List<ILog>();
        private readonly object _sync = new object();

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
            return new AggregateLogger(_logs);
        }
    }
}

