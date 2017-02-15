using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Application
{
    /// <summary>
    /// State of Application. 
    /// </summary>
    public enum AppState
    {
        Undefined = 0,
        Started = 1,
        Running = 2,
        Shutdown = 3,
        Disposed = 4
    }
}
