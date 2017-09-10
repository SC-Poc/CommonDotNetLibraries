using System;
using System.Threading.Tasks;

namespace Common.Log
{
    public class LogToConsole : ILog
    {
        private readonly object _colorSync = new object();

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
            var currentColor = Console.ForegroundColor;
            lock (_colorSync)
            {
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
            var currentColor = Console.ForegroundColor;
            lock (_colorSync)
            {
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
            var currentColor = Console.ForegroundColor;
            lock (_colorSync)
            {
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
