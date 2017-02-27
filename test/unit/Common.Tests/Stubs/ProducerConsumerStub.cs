using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;

namespace Common.Tests.Stubs
{
    /// <summary>
    /// Derived class which helps to test ProducerConsumer<T>
    /// </summary>
    internal class ProducerConsumerStub : ProducerConsumer<string>
    {
        private ConcurrentQueue<string> _consumed = new ConcurrentQueue<string>();

        public IReadOnlyList<string> Consumed { get { return _consumed.ToList(); } }

        public ProducerConsumerStub(string componentName, ILog log)
            : base(componentName, log)
        {
        }

        public void ProduceMessage(string mes)
        {
            this.Produce(mes);
        }

        protected override Task Consume(string item)
        {
            _consumed.Enqueue(item);
            return Task.FromResult(0);
        }
    }
}
