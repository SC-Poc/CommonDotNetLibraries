using System;
using System.Threading.Tasks;

namespace Common.Log
{
    public interface ILog
    {
        /// <summary>
        /// Writes info log message
        /// </summary>
        /// <remarks>
        /// Write an info message about whatever you need to to simplify debugging and maintenance
        /// </remarks>
        Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes monitoring log message
        /// </summary>
        /// <remarks>
        /// Write a monitoring message about app lifecycle events or health events (start, stop, etc.)
        /// </remarks>
        Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <remarks>
        /// Write a warning when something went wrong without any exceptions, and app can still run normally
        /// </remarks>
        Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message with exception
        /// </summary>
        /// <remarks>
        /// Write a warning with exception when you catch an exception but it is not the error for you, and app can still run normally
        /// </remarks>
        Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null);
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <remarks>
        /// Write a error when exception was thrown, but app can still run
        /// </remarks>
        Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes fatal error message
        /// </summary>
        /// <remarks>
        /// Write a fatal error when exception was thrown and app can't still run anymore
        /// </remarks>
        Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes info log message
        /// </summary>
        /// <remarks>
        /// Write an info message about whatever you need to to simplify debugging and maintenance
        /// </remarks>
        Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes monitoring log message
        /// </summary>
        /// <remarks>
        /// Write a monitoring message about app lifecycle events or health events (start, stop, etc.)
        /// </remarks>
        Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <remarks>
        /// Write a warning when something went wrong without any exceptions, and app can still run normally
        /// </remarks>
        Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message with exception
        /// </summary>
        /// <remarks>
        /// Write a warning with exception when you catch an exception but it is not the error for you, and app can still run normally
        /// </remarks>
        Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null);
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <remarks>
        /// Write a error when exception was thrown, but app can still run
        /// </remarks>
        Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes fatal error message
        /// </summary>
        /// <remarks>
        /// Write a fatal error when exception was thrown and app can't still run anymore
        /// </remarks>
        Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null);
    }
}
