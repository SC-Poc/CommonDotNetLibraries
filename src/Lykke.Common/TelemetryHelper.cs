using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Common
{
    /// <summary>
    /// Helper class for ApplicationInsights telemetry submission
    /// </summary>
    public static class TelemetryHelper
    {
        private static readonly TelemetryClient _telemetryClient = new TelemetryClient();

        public static void TrackMetric(string name, double value)
        {
            _telemetryClient.TrackMetric(name, value);
        }

        public static IOperationHolder<RequestTelemetry> StartRequestOperation(string name)
        {
            return _telemetryClient.StartOperation<RequestTelemetry>(name);
        }

        public static void StopOperation<T>(IOperationHolder<T> telemtryOperation)
            where T : OperationTelemetry
        {
            _telemetryClient.StopOperation(telemtryOperation);
        }

        public static void MarkFailedOperation<T>(IOperationHolder<T> telemtryOperation)
            where T : OperationTelemetry
        {
            telemtryOperation.Telemetry.Success = false;
        }

            public static void TrackException(Exception exception)
        {
            _telemetryClient.TrackException(exception);
        }
    }
}
