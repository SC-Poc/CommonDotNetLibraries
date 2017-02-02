using System;
using System.Threading.Tasks;
using Autofac;
using Common.Log;

namespace Common
{
    public abstract class ProducerConsumer<T> : IStartable, IStopable where T:class 
    {
        private readonly string _componentName;
        private readonly ILog _log;

        private readonly AsyncQueue<T> _queue = new AsyncQueue<T>();

        protected abstract Task Consume(T item);

        protected ProducerConsumer(string componentName, ILog log)
        {
            _componentName = componentName;
            _log = log;
        }

        private async Task Handler()
        {
            while (_task != null)
            {
                try
                {
                    var item = await _queue.DequeueAsync();
                    await Consume(item);
                }
                catch (Exception exception)
                {
                    await _log.WriteErrorAsync(_componentName, "Handle", "", exception);
                }
            }
        }

        protected void Produce(T item)
        {
            lock (_queue)
                _queue.Enqueue(item); 
  
            Start();
        }



        private Task _task;

        private readonly object _lockobject = new object();
        public void Start()
        {
            lock (_lockobject)
            {
                if (_task != null)
                    return;
                _task = Handler();
            }
        }


        public void Stop()
        {
            lock (_lockobject)
            {
                if (_task == null)
                    return;

                var task = _task;
                _task = null;
                task.Wait();
            }

        }

    }
}
