using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Common.Log
{
    /// <summary>
    /// Health notifier, that notifies nobody. Could be used in tests
    /// </summary>
    [PublicAPI]
    public class EmptyHealthNotifier : IHealthNotifier
    {
        public static IHealthNotifier Instance { get; } = new EmptyHealthNotifier();

        public Task NotifyAsync(string healthMessage, object context = null)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
