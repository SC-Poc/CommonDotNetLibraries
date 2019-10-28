using System;

namespace Common
{
    /// <summary>
    /// Interace for components that can be stopped.
    /// </summary>
    public interface IStopable : IDisposable
    {
        void Stop();
    }
}
