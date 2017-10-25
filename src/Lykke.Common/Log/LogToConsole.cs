using System;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using Microsoft.Extensions.PlatformAbstractions;

namespace Common.Log
{
    public class LogToConsole : ILog, IConsole
    {
        private readonly string _component;
        private readonly object _colorSync = new object();

        public LogToConsole()
        {
            var app = PlatformServices.Default.Application;
            _component = $"{app.ApplicationName} {app.ApplicationVersion}";
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
            lock (_colorSync)
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
            return exception.ToAsyncString();
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
