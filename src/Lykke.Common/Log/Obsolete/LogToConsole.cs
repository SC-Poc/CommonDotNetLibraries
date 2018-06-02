using System;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace Common.Log
{
    [Obsolete("Use new logging system and call logging.AddLykkeConsole() in startup")]
    public class LogToConsole : ILog, IConsole
    {
        private static readonly object ColorSync = new object();

        private readonly string _component;
        
        public LogToConsole()
        {
            var app = PlatformServices.Default.Application;
            _component = $"{app.ApplicationName} {app.ApplicationVersion}";
        }

        void ILog.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        IDisposable ILog.BeginScope(string scopeMessage)
        {
            throw new NotImplementedException();
        }

        public Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.Gray,
                "INFO",
                component,
                process,
                context,
                info,
                dateTime);
        }

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.White,
                "MONITOR",
                component,
                process,
                context,
                info,
                dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.Yellow,
                "WARNING",
                component,
                process,
                context,
                info,
                dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.Yellow,
                "WARNING",
                component,
                process,
                context,
                $"{info}: {GetExceptionString(ex)}",
                dateTime);
        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.Red,
                "ERROR",
                component,
                process,
                context,
                GetExceptionString(exception),
                dateTime);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return LogMessage(
                ConsoleColor.Red,
                "FATALERROR",
                component,
                process,
                context,
                GetExceptionString(exception),
                dateTime);
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(_component, process, context, info, dateTime);
        }

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteMonitorAsync(_component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteWarningAsync(_component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return WriteWarningAsync(_component, process, context, info, ex, dateTime);
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteErrorAsync(_component, process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteFatalErrorAsync(_component, process, context, exception, dateTime);
        }

        private Task LogMessage(
            ConsoleColor color,
            string level,
            string component,
            string process,
            string context,
            string message,
            DateTime? dateTime)
        {
            lock (ColorSync)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;

                var time = dateTime ?? DateTime.UtcNow;
                Console.WriteLine($"{time:yyyy-MM-dd HH:mm:ss:fff} [{level}] {component}:{process}:{context} - {message}");

                Console.ForegroundColor = oldColor;
            }

            return Task.CompletedTask;
        }

        private static string GetExceptionString(Exception exception)
        {
            return exception?.ToAsyncString();
        }

        public void WriteLine(string line)
        {
            Console.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff} {line}");
        }
    }
}
