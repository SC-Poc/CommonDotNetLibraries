using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Lykke timer implementation
    /// </summary>
    /// <inheritdoc />
    [PublicAPI]
    public sealed class TimerTrigger : ITimerTrigger
    {
        public event TimerTriggeredEventHandler Triggered;

        private readonly DelegatingTimerPeriod _timerPeriod;

        /// <summary>
        /// Creates Lykke timer implementation
        /// </summary>
        /// <param name="componentName">Name of the component, which will be used in the errors logging</param>
        /// <param name="period">Period between <paramref name="handler"/>/<see cref="Triggered"/> executions</param>
        /// <param name="log">The log</param>
        /// <param name="handler">
        /// <p>
        /// Handler which will be trigger every time, when <paramref name="period"/> is ellapsed.
        /// </p>
        /// <p>
        /// This is just shortcut to assign handler for <see cref="Triggered"/> 
        /// </p>
        /// <p>
        /// You do not need to catch exceptions just to for logging in handler of this event,
        /// infrastructure will do it for you.
        /// </p>
        /// </param>
        public TimerTrigger(
            [NotNull] string componentName, 
            TimeSpan period,
            [NotNull] ILog log,
            [NotNull] TimerTriggeredEventHandler handler) :

            this(componentName, period, log)
        {
            Triggered += handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Creates Lykke timer implementation
        /// </summary>
        /// <param name="componentName">Name of the component, which will be used in the errors logging</param>
        /// <param name="period">Period between <see cref="Triggered"/> executions</param>
        /// <param name="log">The log</param>
        public TimerTrigger(
            [NotNull] string componentName, 
            TimeSpan period,
            [NotNull] ILog log)
        {
            if (componentName == null)
            {
                throw new ArgumentNullException(nameof(componentName));
            }
            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period, "Period should be positive time span");
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            _timerPeriod = new DelegatingTimerPeriod(componentName, period, log, OnTriggered);
        }

        /// <summary>
        /// Disables Application Insight telemetry for the timer
        /// </summary>
        public TimerTrigger DisableTelemetry()
        {
            _timerPeriod.DisableTelemetry();

            return this;
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            _timerPeriod.Start();
        }

        /// <summary>
        /// Disposes the timer
        /// </summary>
        public void Dispose()
        {
            _timerPeriod.Dispose();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            _timerPeriod.Stop();
        }

        private Task OnTriggered(CancellationToken cancellationToken)
        {
            return Triggered?.Invoke(this, new TimerTriggeredHandlerArgs(), cancellationToken) ?? Task.CompletedTask;
        }
    }
}
