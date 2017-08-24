using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;

namespace Common.Tests.Stubs
{
    /// <summary>
    /// Derived class which helps to test ProducerConsumer<T>
    /// </summary>
    internal class ProducerConsumerStub : ProducerConsumer<string>
    {
        private readonly List<string> _consumed = new List<string>();

        public IReadOnlyList<string> Consumed => _consumed;

        public ProducerConsumerStub(string componentName, ILog log)
            : base(componentName, log)
        {
        }

        public void ProduceMessage(string mes)
        {
            Produce(mes);
        }

        protected override Task Consume(string item)
        {
            _consumed.Add(item);
            return Task.FromResult(0);
        }
    }
}
