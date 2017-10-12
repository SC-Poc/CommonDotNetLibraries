using System;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Common.RemoteUi;

namespace Common.Log
{
    public static class LogUtils
    {
        public static readonly string[] GuiHeader =
        {
            "Date Time", "Level", "Component", "Process", "Context", "Type", "Msg"
        };
    }

    public class LogToMemory : ILog, IGuiTable
    {
        private readonly GuiTableLastData _guiTableLastData = new GuiTableLastData(50, LogUtils.GuiHeader);
        private readonly string _component;

        public GuiTableData TableData { get { return _guiTableLastData.TableData; } }

        public int Count { get { return _guiTableLastData.Count; } }

        public LogToMemory()
        {
            var app = PlatformServices.Default.Application;
            _component = $"{app.ApplicationName} {app.ApplicationVersion}";
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

        public Task WriteErrorAsync(string component, string process, string context, Exception type, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("error", component, process, context, type.GetType().ToString(), type.GetBaseException().Message, dateTime);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception type, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("fatalerror", component, process, context, type.GetType().ToString(), type.GetBaseException().Message, dateTime);
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

        public Task WriteErrorAsync(string process, string context, Exception type, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("error", _component, process, context, type.GetType().ToString(), type.GetBaseException().Message, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception type, DateTime? dateTime = null)
        {
            return WriteRecordToMemory("fatalerror", _component, process, context, type.GetType().ToString(), type.GetBaseException().Message, dateTime);
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
