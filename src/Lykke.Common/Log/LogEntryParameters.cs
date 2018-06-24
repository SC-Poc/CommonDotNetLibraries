using System;
using JetBrains.Annotations;

namespace Lykke.Common.Log
{
    /// <summary>
    /// Parameters of the log entry
    /// </summary>
    [PublicAPI]
    public class LogEntryParameters
    {
        [NotNull]
        public string AppName { get; }

        [NotNull]
        public string AppVersion { get; }

        [NotNull]
        public string EnvInfo { get; }

        [NotNull]
        public string CallerFilePath { get; }

        [NotNull]
        public string Process { get; }

        public int CallerLineNumber { get; }

        [CanBeNull]
        public string Message { get; }

        [CanBeNull]
        public string Context { get; }

        public DateTime Moment { get; }

        /// <summary>
        /// Creates parameters of the log entry
        /// </summary>
        /// <param name="appName">Name of the app, which made the entry</param>
        /// <param name="appVersion">Version of the app, which made the entry</param>
        /// <param name="envInfo">ENV_INFO environment variable of the app, which made the entry</param>
        /// <param name="callerFilePath">Path of the source code file, where the entry was made</param>
        /// <param name="process">Process within which the entry was made. Usually method name</param>
        /// <param name="callerLineNumber">Source code file line number, where the entry was made</param>
        /// <param name="message">Message of the entry</param>
        /// <param name="context">Context of the entry</param>
        /// <param name="moment">Moment when the entry was made</param>
        public LogEntryParameters(
            [NotNull] string appName,
            [NotNull] string appVersion, 
            [NotNull] string envInfo,
            [NotNull] string callerFilePath,
            [NotNull] string process, 
            int callerLineNumber,
            [CanBeNull] string message, 
            [CanBeNull] object context, 
            [CanBeNull] DateTime? moment)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentException("Should be not empty string", nameof(appName));
            }
            if (string.IsNullOrWhiteSpace(appVersion))
            {
                throw new ArgumentException("Should be not empty string", nameof(appVersion));
            }
            if (string.IsNullOrWhiteSpace(envInfo))
            {
                throw new ArgumentException("Should be not empty string", nameof(envInfo));
            }
            if (string.IsNullOrWhiteSpace(callerFilePath))
            {
                throw new ArgumentException("Should be not empty string", nameof(callerFilePath));
            }
            if (string.IsNullOrWhiteSpace(process))
            {
                throw new ArgumentException("Should be not empty string", nameof(process));
            }
            if (callerLineNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(callerLineNumber), callerLineNumber, "Should be positive number");
            }

            AppName = appName;
            AppVersion = appVersion;
            EnvInfo = envInfo;
            CallerFilePath = callerFilePath;
            Process = process;
            CallerLineNumber = callerLineNumber;
            Message = message;
            Context = LogContextConversion.ConvertToString(context);
            Moment = moment ?? DateTime.UtcNow;
        }
    }
}
