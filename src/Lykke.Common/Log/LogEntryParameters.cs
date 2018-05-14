using System;

namespace Lykke.Common.Log
{
    public class LogEntryParameters
    {
        public string AppName { get; }
        public string AppVersion { get; }
        public string EnvInfo { get; }
        public string Message { get; }
        public object Context { get; }
        public DateTime Moment { get; }

        public LogEntryParameters(
            string appName,
            string appVersion,
            string envInfo,
            string message,
            object context,
            DateTime moment)
        {
            AppName = appName;
            AppVersion = appVersion;
            EnvInfo = envInfo;
            Message = message;
            Context = context;
            Moment = moment;
        }
    }
}
