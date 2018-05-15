using System;
using Common.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Lykke.Common.Log
{
    internal sealed class LogFactory : ILogFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILog CreateLog<TComponent>(TComponent component, string componentNameSuffix)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (string.IsNullOrWhiteSpace(componentNameSuffix))
            {
                throw new ArgumentException("Should be non empty string", nameof(componentNameSuffix));
            }

            return new Log(_loggerFactory.CreateLogger($"{TypeNameHelper.GetTypeDisplayName(component.GetType())}[{componentNameSuffix}]"));
        }

        public ILog CreateLog<TComponent>(TComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return new Log(_loggerFactory.CreateLogger(TypeNameHelper.GetTypeDisplayName(component.GetType())));
        }
    }
}
