using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common.Log;

namespace Common
{
    public abstract class ProducerConsumer<T> : IStartable, IStopable where T : class

    {
        private readonly string _componentName;
        private readonly ILog _log;
        private readonly object _startStopLockobject = new object();

        private readonly Queue<TaskCompletionSource<T>> _queue = new Queue<TaskCompletionSource<T>>();

        protected abstract Task Consume(T item);

        protected ProducerConsumer(string componentName, ILog log)
        {
            _componentName = componentName;
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
                    var task = Dequeue();
                    var value = await task.Task;

                    if (value == null)
                        return;

                    await Consume(value);
                }
                catch (Exception exception)
                {
                    await _log.WriteErrorAsync(_componentName, "Handle", "", exception);
                }
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

        public void Start()
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

        public void Stop()
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
    }


}
