using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Common
{
    /// <summary>
    /// Lykke producer-consumer implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ProducerConsumerTrigger<T> : IProducerConsumerTrigger<T> 
        where T : class
    {
        /// <inheritdoc />
        public event ProducerConsumerConsumedEventHandler<T> Consumed;

        private readonly DelegatingProducerConsumer<T> _producerConsumer;

        /// <summary>
        /// Creates Lykke producer-consumer implementation
        /// </summary>
        /// <param name="componentName">Name of the component, which will be used in the errors logging</param>
        /// <param name="logFactory">The log</param>
        /// <param name="handler">
        /// <p>
        /// Handler which will be trigger every time, when item is consumed.
        /// </p>
        /// <p>
        /// This is just shortcut to assign handler for <see cref="Consumed"/> 
        /// </p>
        /// <p>
        /// You do not need to catch exceptions just to for logging in handler of this event,
        /// infrastructure will do it for you.
        /// </p>
        /// </param>
        /// <param name="enableAppInisghtsMetric">Enables Application Insight telemetry</param>
        public ProducerConsumerTrigger(
            [NotNull] string componentName, 
            [NotNull] ILogFactory logFactory,
            [CanBeNull] ProducerConsumerConsumedEventHandler<T> handler = null,
            bool enableAppInisghtsMetric = false)
        {
            if (componentName == null)
            {
                throw new ArgumentNullException(nameof(componentName));
            }
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            if (handler != null)
            {
                Consumed += handler;
            }

            _producerConsumer = new DelegatingProducerConsumer<T>(logFactory, componentName, OnTriggered, enableAppInisghtsMetric);
        }

        /// <inheritdoc />
        public void Produce(T item)
        {
            _producerConsumer.ProduceItem(item);
        }

        /// <summary>
        /// Starts the producer-consumer
        /// </summary>
        public void Start()
        {
            _producerConsumer.Start();
        }

        /// <summary>
        /// Stops the producer-consumer
        /// </summary>
        public void Stop()
        {
            _producerConsumer.Stop();
        }

        /// <summary>
        /// Disposes the producer-consumer
        /// </summary>
        public void Dispose()
        {
            _producerConsumer.Dispose();
        }

        private Task OnTriggered(T item, CancellationToken cancellationToken)
        {
            return Consumed?.Invoke(this, new ProducerConsumerConsumedHandlerArgs<T>(item), cancellationToken) ?? Task.CompletedTask;
        }
    }
}
