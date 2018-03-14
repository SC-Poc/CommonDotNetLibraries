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
    public static class ApplicationInsightsTelemetry
    {
        private static readonly TelemetryClient _telemetryClient = new TelemetryClient();

        /// <summary>
        /// Submits telemetry of Metric type with rpovided name and value.
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="value">Metric value</param>
        public static void TrackMetric(string name, double value)
        {
            _telemetryClient.TrackMetric(name, value);
        }

        /// <summary>
        /// Initializes telemetry operation of RequestTelemetry type for future submission.
        /// </summary>
        /// <param name="name">Operation name</param>
        /// <returns>Telemetry operation</returns>
        public static IOperationHolder<RequestTelemetry> StartRequestOperation(string name)
        {
            return _telemetryClient.StartOperation<RequestTelemetry>(name);
        }

        /// <summary>
        /// Submits telemetry operation that was created before.
        /// </summary>
        /// <param name="telemtryOperation">Telemetry operation</param>
        public static void StopOperation<T>(IOperationHolder<T> telemtryOperation)
            where T : OperationTelemetry
        {
            _telemetryClient.StopOperation(telemtryOperation);
        }

        /// <summary>
        /// Marks telemetry operation as failed.
        /// </summary>
        /// <param name="telemtryOperation">Telemetry operation</param>
        public static void MarkFailedOperation<T>(IOperationHolder<T> telemtryOperation)
            where T : OperationTelemetry
        {
            telemtryOperation.Telemetry.Success = false;
        }

        /// <summary>
        /// Submits exception telemetry.
        /// </summary>
        /// <param name="exception">Exception</param>
        public static void TrackException(Exception exception)
        {
            _telemetryClient.TrackException(exception);
        }
    }
}
