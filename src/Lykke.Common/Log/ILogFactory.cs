using Common.Log;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.Log
{
    /// <summary>
    /// Log factory abstraction.
    /// Inject this interface into your class, if you need something to log.
    /// </summary>
    [PublicAPI]
    public interface ILogFactory
    {
        /// <summary>
        /// Creates the log for the component.
        /// Call this method right in the ctor of class, which need to log something, and keep obtained log
        /// as private filed of your class.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component</typeparam>
        /// <param name="component">Component instance. Just pass <see langword="this"/>.</param>
        /// <param name="componentNameSuffix">Suffix of the component name.</param>
        [NotNull]
        ILog CreateLog<TComponent>([NotNull] TComponent component, [NotNull] string componentNameSuffix);

        /// <summary>
        /// Creates the log for the component.
        /// Call this method right in the ctor of class, which need to log something, and keep obtained log
        /// as private filed of your class.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component</typeparam>
        /// <param name="component">Component instance. Just pass <see langword="this"/>.</param>
        [NotNull]
        ILog CreateLog<TComponent>([NotNull] TComponent component);

        /// <summary>
        /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
        void AddProvider(ILoggerProvider provider);
    }
}
