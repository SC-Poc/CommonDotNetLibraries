using Autofac;
using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Lykke producer-consumer abstraction.
    /// </summary>
    /// <remarks>
    /// <p>
    /// You do not need to catch exceptions just for logging in handler of this event,
    /// infrastructure will do it for you.
    /// </p>
    /// </remarks>
    [PublicAPI]
    public interface IProducerConsumerTrigger<T> : IStartable, IStopable
    {
        /// <summary>
        /// <p>
        /// Handler which will be trigger every time, when item is consumed.
        /// </p>
        /// <p>
        /// You do not need to catch exceptions just to for logging in handler of this event,
        /// infrastructure will do it for you.
        /// </p>
        /// </summary>
        [CanBeNull]
        event ProducerConsumerConsumedEventHandler<T> Consumed;

        /// <summary>
        /// Produces the item
        /// </summary>
        void Produce(T item);
    }
}
