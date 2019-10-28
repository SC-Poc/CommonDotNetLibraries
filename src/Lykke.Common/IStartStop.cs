using Autofac;
using Common;

namespace Lykke.Common
{
    /// <summary>
    /// Interface for components that cab be started and stopped.
    /// </summary>
    public interface IStartStop : IStartable, IStopable
    {
    }
}
