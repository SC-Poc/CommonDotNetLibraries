using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.Extensions.Logging;

namespace Common.Log
{
    // TODO: Should be moved to the Lykke.Common.Log namespace, when legacy loggin system will be removed

    /// <summary>
    /// Log abstraction. 
    /// Do not inject this interface directly into your class, instead of this, 
    /// inject <see cref="ILoggerFactory"/> and obtain log using one of CreateXXX methods.
    /// </summary>
    [PublicAPI]
    public interface ILog
    {
        /// <summary>
        /// Writes an entry to the log.
        /// This is low level method and usually should not be used directly in the app. 
        /// It's inteded to be used only in some special infrastructural cases and for future extensions.
        /// App developer usually should use one of:
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Trace(ILog, string, object, Exception, DateTime?, string, string, int)"/>,
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Debug(ILog, string, object, Exception, DateTime?, string, string, int)"/>,
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Info(ILog, string, object, Exception, DateTime?, string, string, int)"/>,
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Warning(ILog, string, Exception, object, DateTime?, string, string, int)"/>,
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Error(ILog, string, Exception, object, DateTime?, string, string, int)"/>,
        /// <see cref="MicrosoftLoggingBasedLogExtensions.Critical(ILog, string, Exception, object, DateTime?, string, string, int)"/>
        /// extension methods or they overloads
        /// </summary>
        /// <typeparam name="TState">Entry state type</typeparam>
        /// <param name="logLevel">Log severity level</param>
        /// <param name="eventId">Event ID</param>
        /// <param name="state">Entry state</param>
        /// <param name="exception">Exception</param>
        /// <param name="formatter">Message formatter. Could be used by some of the logger implementations</param>
        void Log<TState>(
            LogLevel logLevel, 
            EventId eventId, 
            [NotNull] TState state, 
            [CanBeNull] Exception exception, 
            [NotNull] Func<TState, Exception, string> formatter)

            where TState : LogEntryParameters;

        /// <summary>
        /// Checks if given <paramref name="logLevel"/> enabled
        /// </summary>
        /// <param name="logLevel">Log severity level to check</param>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// <p>
        /// Begins logging scope. Could be used by some logger implementations
        /// </p>
        /// <p>
        /// All entries, that will be logged inside scope,
        /// will contain <see paramref="scopeMessage"/> as well.
        /// </p>
        /// <p>
        /// Scopes could be nested
        /// </p>
        /// </summary>
        /// <param name="scopeMessage">Scope message</param>
        [NotNull]
        IDisposable BeginScope([NotNull] string scopeMessage);

        #region Obsolete methods

        /// <summary>
        /// Writes info log message
        /// </summary>
        /// <remarks>
        /// Write an info message about whatever you need to to simplify debugging and maintenance
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Info()")]
        Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes monitoring log message
        /// </summary>
        /// <remarks>
        /// Write a monitoring message about app lifecycle events or health events (start, stop, etc.)
        /// </remarks>
        [Obsolete] // TODO
        Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <remarks>
        /// Write a warning when something went wrong without any exceptions, and app can still run normally
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Warning()")]
        Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message with exception
        /// </summary>
        /// <remarks>
        /// Write a warning with exception when you catch an exception but it is not the error for you, and app can still run normally
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Warning()")]
        Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null);
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <remarks>
        /// Write a error when exception was thrown, but app can still run
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Error()")]
        Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes fatal error message
        /// </summary>
        /// <remarks>
        /// Write a fatal error when exception was thrown and app can't still run anymore
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Error()")]
        Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes info log message
        /// </summary>
        /// <remarks>
        /// Write an info message about whatever you need to to simplify debugging and maintenance
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Info()")]
        Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes monitoring log message
        /// </summary>
        /// <remarks>
        /// Write a monitoring message about app lifecycle events or health events (start, stop, etc.)
        /// </remarks>
        [Obsolete] // TODO
        Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <remarks>
        /// Write a warning when something went wrong without any exceptions, and app can still run normally
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Warning()")]
        Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null);
        /// <summary>
        /// Writes warning message with exception
        /// </summary>
        /// <remarks>
        /// Write a warning with exception when you catch an exception but it is not the error for you, and app can still run normally
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Warning()")]
        Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null);
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <remarks>
        /// Write a error when exception was thrown, but app can still run
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Error()")]
        Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null);
        /// <summary>
        /// Writes fatal error message
        /// </summary>
        /// <remarks>
        /// Write a fatal error when exception was thrown and app can't still run anymore
        /// </remarks>
        [Obsolete("Use overloads of the new extension methods: Critical()")]
        Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null);

        #endregion
    }
}
