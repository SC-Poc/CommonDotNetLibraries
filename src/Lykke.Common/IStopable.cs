
using System;

namespace Common
{
    public interface IStopable : IDisposable
    {
        void Stop();
    }
}
