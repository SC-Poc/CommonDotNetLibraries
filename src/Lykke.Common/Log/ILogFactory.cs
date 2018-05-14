using Common.Log;

namespace Lykke.Common.Log
{
    public interface ILogFactory
    {
        ILog CreateLog(string componentName);
    }
}
