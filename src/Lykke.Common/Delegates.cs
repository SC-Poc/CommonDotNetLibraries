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
}
