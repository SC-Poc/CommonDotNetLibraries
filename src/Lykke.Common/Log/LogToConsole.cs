using System;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace Common.Log
{
    public class LogToConsole : ILog
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
            lock(_colorSync)
                LogMessage(
                    "INFO",
                    component,
                    process,
                    context,
                    info,
                    dateTime);
            return Task.FromResult(0);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            lock (_colorSync)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                LogMessage(
                    "WARNING",
                    component,
                    process,
                    context,
                    info,
                    dateTime);
                Console.ForegroundColor = currentColor;
            }
            return Task.FromResult(0);
        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            lock (_colorSync)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage(
                    "ERROR",
                    component,
                    process,
                    context,
                    GetExceptionString(exception),
                    dateTime);
                Console.ForegroundColor = currentColor;
            }
            return Task.FromResult(0);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            lock (_colorSync)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage(
                    "FATALERROR",
                    component,
                    process,
                    context,
                    GetExceptionString(exception),
                    dateTime);
                Console.ForegroundColor = currentColor;
            }
            return Task.FromResult(0);
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(_component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteWarningAsync(_component, process, context, info, dateTime);
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteErrorAsync(_component, process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteFatalErrorAsync(_component, process, context, exception, dateTime);
        }

        private void LogMessage(
            string messageType,
            string component,
            string process,
            string context,
            string message,
            DateTime? dateTime)
        {
            DateTime time = dateTime ?? DateTime.UtcNow;
            Console.WriteLine($"{time.ToString("yyyy-MM-dd HH:mm:ss:fff")} [{messageType}] {component}:{process}:{context} - {message}");
        }

        private string GetExceptionString(Exception exception)
        {
            return exception.ToString();
        }
    }
}
