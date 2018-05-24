using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// <see cref="ITimerTrigger.Triggered"/> handler delegate
    /// </summary>
    /// <param name="timer">Timer being triggered</param>
    /// <param name="args">Event arguments</param>
    /// <param name="cancellationToken">Cancellation token, which will be cancelled, if <see cref="TimerTrigger.Stop"/> is being called</param>
    [PublicAPI]
    public delegate Task TimerTriggeredEventHandler([NotNull] ITimerTrigger timer, [NotNull] TimerTriggeredHandlerArgs args, CancellationToken cancellationToken);

    /// <summary>
    /// <see cref="IProducerConsumerTrigger{T}.Consumed"/> handler delegate
    /// </summary>
    /// <typeparam name="T">Type of the item to consume</typeparam>
    /// <param name="producer">Producer-consumer, which has triggered consume of the item</param>
    /// <param name="args">Event arguments</param>
    /// <param name="cancellationToken">Cancellation token, which will be cancelled, if <see cref="IStopable.Stop"/> is being called</param>
    /// <returns></returns>
    [PublicAPI]
    public delegate Task ProducerConsumerConsumedEventHandler<T>([NotNull] IProducerConsumerTrigger<T> producer, ProducerConsumerConsumedHandlerArgs<T> args, CancellationToken cancellationToken);
}
