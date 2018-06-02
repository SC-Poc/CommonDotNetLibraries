using System;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Common.RemoteUi;
using Microsoft.Extensions.Logging;

namespace Common.Log
{
    public static class LogUtils
    {
        public static string[] GuiHeader => new[]
        {
            "Date Time", "Level", "Component", "Process", "Context", "Type", "Msg"
        };
    }

    [Obsolete("Use new logging system and call logging.AddGuiTable() in startup")]
    public class LogToMemory : ILog, IGuiTable
    {
        private readonly GuiTableLastData _guiTableLastData = new GuiTableLastData(50, LogUtils.GuiHeader);
        private readonly string _component;

        public GuiTableData TableData => _guiTableLastData.TableData;

        public int Count => _guiTableLastData.Count;

        public LogToMemory()
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
            return WriteRecordToMemory("info", component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("monitor", component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("warning", component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("warning", component, process, context, ex.GetType().ToString(), $"{info}: {ex.GetBaseException().Message}", dateTime);

        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("error", component, process, context, exception.GetType().ToString(), exception.GetBaseException().Message, dateTime);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("fatalerror", component, process, context, exception.GetType().ToString(), exception.GetBaseException().Message, dateTime);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("info", _component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("monitor", _component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("warning", _component, process, context, string.Empty, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, Exception ex, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("warning", _component, process, context, ex.GetType().ToString(), $"{info}: {ex.GetBaseException().Message}", dateTime);
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("error", _component, process, context, exception.GetType().ToString(), exception.GetBaseException().Message, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("fatalerror", _component, process, context, exception.GetType().ToString(), exception.GetBaseException().Message, dateTime);
        }

        private Task WriteRecordToMemory(string level, string component, string process,
            string context, string type, string msg, DateTime? dateTime)
        {
            if (dateTime == null)
                dateTime = DateTime.UtcNow;
            _guiTableLastData.NewData(
                dateTime.Value.ToString(Utils.IsoDateTimeMask),
                level,
                component,
                process,
                context,
                type,
                msg);

            return Task.CompletedTask;
        }
    }
}
