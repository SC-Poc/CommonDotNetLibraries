using Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log
{
    public class LogFactory : ILogFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILog CreateLog(string componentName)
        {
            return new Log(_loggerFactory.CreateLogger(componentName));
        }
    }
}
