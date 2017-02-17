using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common.Log;

namespace Common
{
    public abstract class ProducerConsumer<T> : IStartable, IStopable where T:class 
    {
        private readonly string _componentName;
        private readonly ILog _log;
        private readonly ManualResetEventSlim _done = new ManualResetEventSlim(false);
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private volatile CancellationTokenSource ts = null;
        private readonly object _lockobject = new object();

        protected abstract Task Consume(T item);

        protected ProducerConsumer(string componentName, ILog log)
        {
            _componentName = componentName;
            _log = log;
        }

        private void StartThread()
        {
            _done.Reset();

            ts = new CancellationTokenSource();
            var token = ts.Token;

            Task t = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        T item = null;
                        if (_queue.TryDequeue(out item))
                        {
                            await Consume(item);
                        }
                    }
                    catch (Exception exception)
                    {
                        await _log.WriteErrorAsync(_componentName, "Handle", "", exception);
                    }
                }

                _done.Set();
            });
        }

        protected void Produce(T item)
        {
            _queue.Enqueue(item); 
  
            Start();
        }

        public void Start()
        {
            if (ts == null)
            {
                lock (_lockobject)
                {
                    if (ts == null)
                    {
                        StartThread();
                    }
                }
            }
        }

        public void Stop()
        {
            if (ts != null)
            {
                lock (_lockobject)
                {
                    if (ts != null)
                    {
                        ts.Cancel();
                        ts = null;
                    }
                    _done.Wait();
                }
            }
        }
    }
}
