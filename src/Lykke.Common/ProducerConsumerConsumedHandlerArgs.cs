using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Arguments for <see cref="ProducerConsumerConsumedEventHandler{T}"/>
    /// </summary>
    [PublicAPI]
    public class ProducerConsumerConsumedHandlerArgs<T>
    {
        /// <summary>
        /// Produced item, which should be consumed
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// Arguments for <see cref="ProducerConsumerConsumedEventHandler{T}"/>
        /// </summary>
        public ProducerConsumerConsumedHandlerArgs(T item)
        {
            Item = item;
        }
    }
}
