using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Autofac;
using Common.Log;

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

        protected TimerPeriod(string componentName, int periodMs, ILog log = null)
        {
            _componentName = componentName;
            _periodMs = periodMs;
            _typeName = GetType().Name;
            Log = log;
        }

        protected TimerPeriod(int periodMs, ILog log = null)
        {
            _componentName = PlatformServices.Default.Application.ApplicationName;
            _periodMs = periodMs;
            _typeName = GetType().Name;
            Log = log;
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
                throw new Exception(
                    "Logger has to be inited" + (string.IsNullOrWhiteSpace(_componentName) ? "" : $" for: {_componentName}"));

            if (Working)
                return;

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

            _cancellation.Cancel();

            _task?.Wait();
            _task = null;
            _cancellation = null;

            Working = false;
        }

        public string GetComponentName()
        {
            return _componentName ?? "";
        }

        public void Dispose()
        {
            Stop();
        }

        protected void SetLogger(ILog log)
        {
            Log = log;
        }

        protected void DisableTelemetry()
        {
            _isTelemetryDisabled = true;
        }

        private async Task LogFatalErrorAsync(Exception exception)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_componentName))
                    await Log.WriteErrorAsync("Loop", "", exception);
                else
                    await Log.WriteErrorAsync(_componentName, "Loop", "", exception);
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
                            await Execute(cancellation);
                        }
                        catch (Exception exception)
                        {
                            await LogFatalErrorAsync(exception);
                        }
                    }
                    else
                    {
                        var telemtryOperation = ApplicationInsightsTelemetry.StartRequestOperation($"{nameof(TimerPeriod)} on {_typeName} for {_componentName}");
                        try
                        {
                            await Execute(cancellation);
                        }
                        catch (Exception exception)
                        {
                            await LogFatalErrorAsync(exception);
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
                        await Task.Delay(_periodMs, cancellation);
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
