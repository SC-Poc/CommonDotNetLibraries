using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.RemoteUi;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log.Loggers
{
    /// <inheritdoc />
    /// <summary>
    /// Logger, which logs entries to the <see cref="T:Common.RemoteUi.GuiTableData" />
    /// </summary>
    [PublicAPI]
    public sealed class GuiTableLogger : ILogger
    {
        private readonly ISet<LogLevel> _enabledLevels;
        private readonly GuiTableLastData _guiTableLastData;
        private readonly string _component;

        /// <summary>
        /// Logged entries
        /// </summary>
        [NotNull]
        public GuiTableData Entries => _guiTableLastData.TableData;

        /// <summary>
        /// Logged entries count
        /// </summary>
        public int EntriesCount => _guiTableLastData.Count;

        /// <summary>
        /// Creates logger, which logs entries to the <see cref="T:Common.RemoteUi.GuiTableData" />
        /// </summary>
        /// <param name="maxCount">Maximum stored entries</param>
        /// <param name="enabledLevels">Enabled logging levels. <see langword="null"/> means that all levels are enabled</param>
        public GuiTableLogger(int maxCount = 50, [CanBeNull] ISet<LogLevel> enabledLevels = null)
        {
            _enabledLevels = enabledLevels ?? LogLevels.All;
            _guiTableLastData = new GuiTableLastData(maxCount, "Date Time", "Level", "Component", "Process", "Context", "Type", "Msg");
        }

        void ILogger.Write(
            string appName, 
            string appVersion, 
            string envInfo, 
            string component, 
            string process, 
            LogLevel level,
            string message, 
            object context, 
            Exception exception, 
            DateTime moment)
        {
            if (!_enabledLevels.Contains(level))
            {
                return;
            }

            var messageBuilder = new StringBuilder();

            messageBuilder.Append(message);

            if (exception != null)
            {
                messageBuilder.Append(": ");
                messageBuilder.Append(GetExceptionMessage(exception));
            }

            _guiTableLastData.NewData(
                moment.ToString(Utils.IsoDateTimeMask),
                level.ToString(),
                component,
                process,
                LogContextConversion.ConvertToString(context),
                exception?.GetType().ToString(),
                messageBuilder.ToString());
        }

        [NotNull]
        private string GetExceptionMessage(Exception exception)
        {
            var ex = exception;
            var sb = new StringBuilder();

            while (true)
            {
                if (ex.InnerException != null)
                {
                    sb.AppendLine(ex.Message);
                }
                else
                {
                    sb.Append(ex.Message);
                }

                ex = ex.InnerException;

                if (ex == null)
                {
                    return sb.ToString();
                }

                sb.Append(" -> ");
            }
        }
    }
}
