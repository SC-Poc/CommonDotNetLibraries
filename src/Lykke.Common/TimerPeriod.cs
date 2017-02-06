using System;
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

        private async Task ThreadMethod()
        {
            while (Working)
            {
                try
                {
                    await Execute();
                }
                catch (Exception exception)
                {
                    LogFatalError(exception);
                }
                await Task.Delay(_periodMs);
            }
        }

        public virtual void Start()
        {
            if (_log == null)
                throw new Exception("Logger has to be inited for: "+ _componentName);

            if (Working)
                return;

            Working = true;
            _task = ThreadMethod();

        }

        public void Stop()
        {

            Working = false;
            var task = _task;
            _task = null;

            task?.Wait();
        }

        public string GetComponentName()
        {
            return _componentName;
        }
    }
}
