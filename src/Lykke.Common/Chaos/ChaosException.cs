using System;
using JetBrains.Annotations;

namespace Lykke.Common.Chaos
{
    /// <summary>
    /// Exception that is thrown, when <see cref="IChaosKitty"/> is meowing
    /// </summary>
    [PublicAPI]
    public class ChaosException : Exception
    {
        /// <summary>
        /// Exception that is thrown, when <see cref="IChaosKitty"/> is meowing
        /// </summary>
        public ChaosException(string message) :
            base(message)
        {
        }
    }
}
