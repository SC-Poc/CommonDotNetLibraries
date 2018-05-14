using System;
using JetBrains.Annotations;

namespace Common.Log
{
    [Obsolete("Use new Lykke.Common.Log.ILogger + Lykke.Common.Log.ApplicationLog + Lykke.Common.Log.ComponentLog + Lykke.Common.Log.ProcessLog")]
    [PublicAPI]
    public static class LogExtensions
    {
        /// <summary>
        /// Creates component scoped log
        /// </summary>
        /// <remarks>
        /// You can use it when you need to specify the same component for group of log writes.
        /// If you specify component in the particular log write, it will be concatenated with <paramref name="component"/>
        /// </remarks>
        /// <param name="log">Log to wrap</param>
        /// <param name="component">Component name for which scope will be created</param>
        /// <returns></returns>
        public static ILog CreateComponentScope(this ILog log, string component)
        {
            return new LogComponentScope(component, log);
        }

        /// <summary>
        /// Writes info log message
        /// </summary>
        /// <remarks>
        /// Write an info message about whatever you need to to simplify debugging and maintenance.
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteInfo(this ILog log, string process, object context, string info, DateTime? dateTime = null)
        {
            log.WriteInfoAsync(process, GetContextString(context), info, dateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes monitoring log message
        /// </summary>
        /// <remarks>
        /// Write a monitoring message about app lifecycle events or health events (start, stop, etc.).
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteMonitor(this ILog log, string process, object context, string info, DateTime? dateTime = null)
        {
            log.WriteMonitorAsync(process, GetContextString(context), info, dateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <remarks>
        /// Write a warning when something went wrong without any exceptions, and app can still run normally.
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteWarning(this ILog log, string process, object context, string info, DateTime? dateTime = null)
        {
            log.WriteWarningAsync(process, GetContextString(context), info, dateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes warning message with exception
        /// </summary>
        /// <remarks>
        /// Write a warning with exception when you catch an exception but it is not the error for you, and app can still run normally.
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteWarning(this ILog log, string process, object context, string info, Exception ex, DateTime? dateTime = null)
        {
            log.WriteWarningAsync(process, GetContextString(context), info, ex, dateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes error message
        /// </summary>
        /// <remarks>
        /// Write a error when exception was thrown, but app can still run.
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteError(this ILog log, string process, object context, [CanBeNull] Exception exception = null, DateTime? dateTime = null)
        {
            log.WriteErrorAsync(process, GetContextString(context), exception, dateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes fatal error message
        /// </summary>
        /// <remarks>
        /// Write a fatal error when exception was thrown and app can't still run anymore.
        /// If <paramref name="context"/> is string, it will be passed as is, otherwise it will be converted to the Json
        /// </remarks>
        public static void WriteFatalError(this ILog log, string process, object context, [CanBeNull] Exception exception = null, DateTime? dateTime = null)
        {
            log.WriteFatalErrorAsync(process, GetContextString(context), exception, dateTime).GetAwaiter().GetResult();
        }

        private static string GetContextString(object context)
        {
            if (context is string str)
            {
                return str;
            }

            return context?.ToJson();
        }
    }
}
