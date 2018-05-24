using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Common
{
    /// <summary>
    /// Inteded to implement <see cref="ProducerConsumerTrigger{T}"/> without exposing method <see cref="ProducerConsumer{T}.Consume(T)"/> 
    /// and with exposing method <see cref="ProducerConsumer{T}.Produce"/>
    /// </summary>
    internal sealed class DelegatingProducerConsumer<T> : ProducerConsumer<T> where T : class
    {
        private readonly Func<T, CancellationToken, Task> _consume;

        public DelegatingProducerConsumer(
            [NotNull] ILogFactory logFactory, 
            [NotNull] string componentName,
            [NotNull] Func<T, CancellationToken, Task> consume,
            bool enableAppInisghtsMetric) : 
            
            base(logFactory, componentName, enableAppInisghtsMetric)
        {
            _consume = consume ?? throw new ArgumentNullException(nameof(consume));
        }

        public void ProduceItem(T item)
        {
            Produce(item);
        }

        protected override Task Consume(T item, CancellationToken cancellationToken)
        {
            return _consume(item, cancellationToken);
        }
    }
}
