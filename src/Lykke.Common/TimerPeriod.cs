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
    /// Timer that runs Execute method in a loop with a fixed time interval between runs.
    /// </summary>
    public abstract class TimerPeriod : IStartable, IStopable, ITimerCommand
    {
        private readonly string _componentName;
        private readonly int _periodMs;

        protected ILog Log { get; private set; }

        protected TimerPeriod(string componentName, int periodMs, ILog log = null)
        {
            _componentName = componentName;

            _periodMs = periodMs;
            Log = log;
        }

        protected TimerPeriod(int periodMs, ILog log = null)
        {
            _periodMs = periodMs;
            Log = log;
            _componentName = PlatformServices.Default.Application.ApplicationName;
        }

        protected void SetLogger(ILog log)
        {
            Log = log;
        }

        public bool Working { get; private set; }
        private Task _task;
        private CancellationTokenSource _cancellation; 

        private async Task LogFatalErrorAsync(Exception exception)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_componentName))
                    await Log.WriteFatalErrorAsync("Loop", "", exception);
                else
                    await Log.WriteFatalErrorAsync(_componentName, "Loop", "", exception);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        public abstract Task Execute();

        public virtual Task Execute(CancellationToken cancellation)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Execute();
        }

        private async Task ThreadMethod(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    var telemtryOperation = TelemetryHelper.StartRequestOperation($"{nameof(TimerPeriod)} for {_componentName}");
                    try
                    {
                        await Execute(cancellation);
                    }
                    catch (Exception exception)
                    {
                        await LogFatalErrorAsync(exception);
                        TelemetryHelper.MarkFailedOperation(telemtryOperation);
                        TelemetryHelper.TrackException(exception);
                    }
                    finally
                    {
                        TelemetryHelper.StopOperation(telemtryOperation);
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
    }
}
