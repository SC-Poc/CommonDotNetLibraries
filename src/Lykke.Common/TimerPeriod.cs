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

        protected void SetLogger(ILog log)
        {
            _log = log;
        }

        public bool Working { get; private set; }
        private Task _task;
        private CancellationTokenSource _cancellation; 

        private void LogFatalError(Exception exception)
        {
            try
            {
                _log?.WriteFatalErrorAsync(_componentName, "Loop", "", exception).Wait();
            }
            catch (Exception)
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
                    await Execute(cancellation);
                }
                catch (Exception exception)
                {
                    LogFatalError(exception);
                }

                try
                {
                    await Task.Delay(_periodMs, cancellation);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        public virtual void Start()
        {
            if (_log == null)
                throw new Exception("Logger has to be inited for: "+ _componentName);

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
            return _componentName;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
