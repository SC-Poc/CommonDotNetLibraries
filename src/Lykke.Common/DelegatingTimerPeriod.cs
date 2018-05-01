using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;

namespace Common
{
    /// <summary>
    /// Inteded to implement <see cref="TimerTrigger"/> without exposing public method <see cref="TimerPeriod.Execute()"/> 
    /// </summary>
    internal sealed class DelegatingTimerPeriod : TimerPeriod
    {
        private readonly Func<CancellationToken, Task> _execute;

        public DelegatingTimerPeriod(
            string componentName, 
            TimeSpan period, 
            ILog log, 
            Func<CancellationToken, Task> execute) : 

            base(componentName, (int)period.TotalMilliseconds, log)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public override Task Execute(CancellationToken cancellationToken)
        {
            return _execute(cancellationToken);
        }

        public new void DisableTelemetry()
        {
            base.DisableTelemetry();
        }
    }
}
