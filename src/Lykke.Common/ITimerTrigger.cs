using Autofac;
using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Lykke timer abstraction.
    /// </summary>
    /// <remarks>
    /// <p>
    /// You do not need to catch exceptions just to for logging in handler of this event,
    /// infrastructure will do it for you.
    /// </p>
    /// <p>
    /// Handler execution time is not included in the timer period.
    /// </p>
    /// </remarks>
    [PublicAPI]
    public interface ITimerTrigger : IStartable, IStopable
    {
        /// <summary>
        /// This event will be triggered when timer period is ellapsed.
        /// <p>
        /// You do not need to catch exceptions just for logging in handler of this event,
        /// infrastructure will do it for you.
        /// </p>
        /// <p>
        /// Handler execution time is not included in the timer period.
        /// </p>
        /// </summary>
        [CanBeNull]
        event TimerTriggeredEventHandler Triggered;
    }
}
