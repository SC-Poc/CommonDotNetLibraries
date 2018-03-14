using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Common;
using Common.Log;

namespace Lykke.Common
{
    /// <summary>Autofac <see cref="ContainerBuilder"/> extensions for the <see cref="ResourcesMonitor"/></summary>
    [PublicAPI]
    public static class ResourcesMonitorExtensions
    {
        /// <summary>
        /// Registers <see cref="ResourcesMonitor"/> singleton with ApplicationInsights telemetry submission only"/>
        /// </summary>
        /// <param name="builder">The DI container builder</param>
        public static void RegisterResourcesMonitoring([NotNull] this ContainerBuilder builder)
        {
            builder.RegisterType<ResourcesMonitor>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }

        /// <summary>
        /// Registers <see cref="ResourcesMonitor"/> singleton that beside ApplicationInsights telemetry submission also logs threshold crossing events on monitor level"/>
        /// </summary>
        /// <param name="builder">The DI container builder</param>
        /// <param name="cpuThreshold">Optional CPU threshold for monitor logging</param>
        /// <param name="ramMbThreshold">Optional RAM threshold for monitor logging</param>
        public static void RegisterResourcesMonitoringWithLogging([NotNull] this ContainerBuilder builder, double? cpuThreshold, int? ramMbThreshold)
        {
            builder.RegisterType<ResourcesMonitor>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(cpuThreshold))
                .WithParameter(TypedParameter.From(ramMbThreshold));
        }
    }

    /// <summary>
    /// CPU and RAM monitoring background checker based on TimerPeriod with once per minute frequency
    /// </summary>
    public sealed class ResourcesMonitor : TimerPeriod
    {
        private const double _1mb = 1024 * 1024;
        private const string _cpuMetric = "Custom CPU";
        private const string _ramMetric = "Custom RAM";

        private readonly ILog _log;
        private readonly Process _process = Process.GetCurrentProcess();
        private readonly Stopwatch _cpuWatch = new Stopwatch();
        private readonly double? _cpuThreshold;
        private readonly int? _ramMbThreshold;
        private readonly TimeSpan _startCpuTime;

        /// <summary>
        /// Inits monitoring with ApplicationInsights telemetry submission only.
        /// </summary>
        /// <param name="log">ILog implementation</param>
        public ResourcesMonitor(ILog log)
            : base((int)TimeSpan.FromMinutes(1).TotalMilliseconds, log)
        {
            _startCpuTime = _process.TotalProcessorTime;
            _cpuWatch.Start();
        }

        /// <summary>
        /// Inits monitoring that beside ApplicationInsights telemetry submission also logs threshold crossing events on monitor level.
        /// </summary>
        /// <param name="log">ILog implementation</param>
        /// <param name="cpuThreshold">Optional CPU threshold for monitor logging</param>
        /// <param name="ramMbThreshold">Optional RAM threshold for monitor logging</param>
        public ResourcesMonitor(ILog log, double? cpuThreshold, int? ramMbThreshold)
            : base((int)TimeSpan.FromMinutes(1).TotalMilliseconds, log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _cpuThreshold = cpuThreshold;
            _ramMbThreshold = ramMbThreshold;
            _startCpuTime = _process.TotalProcessorTime;
            _cpuWatch.Start();
        }

        public override Task Execute()
        {
            // A very simple and not that accruate evaluation of how much CPU the process is take out of a core.
            double cpuPercentage = (_process.TotalProcessorTime - _startCpuTime).TotalMilliseconds / _cpuWatch.ElapsedMilliseconds;
            ApplicationInsightsTelemetry.TrackMetric(_cpuMetric, cpuPercentage);

            if (_cpuThreshold.HasValue && _cpuThreshold.Value <= cpuPercentage)
                _log.WriteMonitor(nameof(ResourcesMonitor), "", $"CPU usage is {cpuPercentage}");

            double memoryInMBytes = _process.WorkingSet64 / _1mb;
            ApplicationInsightsTelemetry.TrackMetric(_ramMetric, memoryInMBytes);

            if (_ramMbThreshold.HasValue && _ramMbThreshold.Value <= memoryInMBytes)
                _log.WriteMonitor(nameof(ResourcesMonitor), "", $"RAM usage is {memoryInMBytes}");

            return Task.CompletedTask;
        }
    }
}
