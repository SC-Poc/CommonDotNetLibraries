using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Autofac;
using Common.Log;
using JetBrains.Annotations;

namespace Common
{
    public interface ITimerCommand
    {
        Task Execute();
    }

    /// <summary>
    /// Timer that runs Execute method in a loop with a fixed time interval between runs (Execute method execution time is not included).
    /// </summary>
    public abstract class TimerPeriod : IStartable, IStopable, ITimerCommand
    {
        private readonly string _componentName;
        private readonly int _periodMs;
        private readonly string _typeName;

        private Task _task;
        private CancellationTokenSource _cancellation;
        private bool _isTelemetryDisabled;

        protected ILog Log { get; private set; }

        public bool Working { get; private set; }

        protected TimerPeriod(
            [CanBeNull] string componentName, 
            int periodMs, 
            ILog log = null)
        {
            _componentName = componentName ?? PlatformServices.Default.Application.ApplicationName;
            _periodMs = periodMs;
            _typeName = GetType().Name;

            Log = log?.CreateComponentScope(_componentName);
        }

        protected TimerPeriod(
            int periodMs, 
            ILog log) :

            this(null, periodMs, log)
        {
        }

        public virtual Task Execute()
        {
            return Task.CompletedTask;
        }

        public virtual Task Execute(CancellationToken cancellation)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Execute();
        }

        public virtual void Start()
        {
            if (Log == null)
            {
                throw new Exception("Logger has to be inited");
            }

            if (Working)
            {
                return;
            }

            Working = true;

            _cancellation = new CancellationTokenSource();

            _task = ThreadMethod(_cancellation.Token);
        }

        public virtual void Stop()
        {
            if (!Working)
            {
                return;
            }

            _cancellation?.Cancel();
            _cancellation?.Dispose();

            _task?.ConfigureAwait(false).GetAwaiter().GetResult();
            _task?.Dispose();

            _task = null;
            _cancellation = null;

            Working = false;
        }

        public string GetComponentName()
        {
            return _componentName;
        }

        public void Dispose()
        {
            Stop();
        }

        [Obsolete("Pass log to the ctor")]
        protected void SetLogger(ILog log)
        {
            Log = log?.CreateComponentScope(_componentName);
        }

        protected void DisableTelemetry()
        {
            _isTelemetryDisabled = true;
        }

        private void LogFatalError(Exception exception)
        {
            try
            {
                Log.WriteError("Loop", "", exception);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private async Task ThreadMethod(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    if (_isTelemetryDisabled)
                    {
                        try
                        {
                            await Execute(cancellation).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            LogFatalError(exception);
                        }
                    }
                    else
                    {
                        var telemtryOperation = ApplicationInsightsTelemetry.StartRequestOperation($"{nameof(TimerPeriod)} on {_typeName} for {_componentName}");
                        try
                        {
                            await Execute(cancellation).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            LogFatalError(exception);
                            ApplicationInsightsTelemetry.MarkFailedOperation(telemtryOperation);
                            ApplicationInsightsTelemetry.TrackException(exception);
                        }
                        finally
                        {
                            ApplicationInsightsTelemetry.StopOperation(telemtryOperation);
                        }
                    }

                    try
                    {
                        await Task.Delay(_periodMs, cancellation).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
                // Saves the loop if nothing didn't help
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }
    }
}
