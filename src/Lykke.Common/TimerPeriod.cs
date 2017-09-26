using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common.Log;

namespace Common
{
    public interface ITimerCommand
    {
        Task Execute();
    }

    // Таймер, который исполняет метод Execute через определенный интервал после окончания исполнения метода Execute
    public abstract class TimerPeriod : IStartable, IStopable, ITimerCommand
    {
        private readonly string _componentName;
        private readonly int _periodMs;
        private ILog _log;

        protected TimerPeriod(string componentName, int periodMs, ILog log = null)
        {
            _componentName = componentName;

            _periodMs = periodMs;
            _log = log;
        }

        protected TimerPeriod(int periodMs, ILog log = null)
        {
            _periodMs = periodMs;
            _log = log;
        }

        protected void SetLogger(ILog log)
        {
            _log = log;
        }

        public bool Working { get; private set; }
        private Task _task;
        private CancellationTokenSource _cancellation; 

        private async Task LogFatalErrorAsync(Exception exception)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_componentName))
                    await _log.WriteFatalErrorAsync("Loop", "", exception);
                else
                    await _log.WriteFatalErrorAsync(_componentName, "Loop", "", exception);
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
                    try
                    {
                        await Execute(cancellation);
                    }
                    catch (Exception exception)
                    {
                        await LogFatalErrorAsync(exception);
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
            if (_log == null)
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
