using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common.Log;

namespace Common
{
    public abstract class ProducerConsumer<T> : IStartable, IStopable where T : class
    {
        protected readonly string _componentName;
        private readonly ILog _log;
        private readonly object _startStopLockobject = new object();

        private readonly Queue<TaskCompletionSource<T>> _queue = new Queue<TaskCompletionSource<T>>();

        protected abstract Task Consume(T item);

        protected ProducerConsumer(string componentName, ILog log)
        {
            _componentName = componentName;
            _log = log;
        }

        protected ProducerConsumer(ILog log)
        {
            _log = log;
        }

        private bool _started;
        private Task _threadTask;
        private TaskCompletionSource<T> _last;

        private TaskCompletionSource<T> Dequeue()
        {
            lock (_queue)
                return _queue.Dequeue();
        }

        private async Task StartTask()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        var task = Dequeue();
                        var value = await task.Task;

                        if (value == null)
                            return;

                        await Consume(value);
                    }
                    catch (Exception exception)
                    {
                        await LogErrorAsync(exception);
                    }
                }
                // Saves the loop if nothing didn't help
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }

        private async Task LogErrorAsync(Exception exception)
        {
            try
            {
                if (_log != null)
                {
                    if (string.IsNullOrWhiteSpace(_componentName))
                        await _log.WriteErrorAsync("Handle", "", exception);
                    else
                        await _log.WriteErrorAsync(_componentName, "Handle", "", exception);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        protected void Produce(T item)
        {
            Start();

            lock (_queue)
            {
                var last = _last;
                _last = new TaskCompletionSource<T>();
                _queue.Enqueue(_last);
                last.SetResult(item);
            }

        }

        public virtual void Start()
        {
            if (_started)
                return;

            lock (_startStopLockobject)
            {
                if (_started)
                    return;

                _started = true;
            }


            _last = new TaskCompletionSource<T>();
            lock (_queue)
                _queue.Enqueue(_last);

            _threadTask = StartTask();
        }

        public virtual void Stop()
        {
            if (!_started)
                return;

            lock (_startStopLockobject)
            {
                if (!_started)
                    return;
                _started = false;
            }

            _last.SetResult(null);
            _threadTask.Wait();
        }

        public void Dispose()
        {
            Stop();
        }
    }


}
