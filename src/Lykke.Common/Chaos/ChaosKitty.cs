using System;
using JetBrains.Annotations;

namespace Lykke.Common.Chaos
{
    /// <inheritdoc />
    [PublicAPI]
    public class ChaosKitty : IChaosKitty
    {
        private readonly Random _randmom;
        private readonly double _stateOfChaos;

        public ChaosKitty(double stateOfChaos)
        {
            if (stateOfChaos < 0.0 || stateOfChaos > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(stateOfChaos), stateOfChaos, "Should be in the range [0, 1]");
            }

            _stateOfChaos = stateOfChaos;

            _randmom = new Random();
        }

        /// <inheritdoc />
        public void Meow(object tag)
        {
            if (_stateOfChaos < 1e-10)
            {
                return;
            }

            if (_randmom.NextDouble() < _stateOfChaos)
            {
                throw new ChaosException($"Meow: {tag}");
            }
        }
    }
}
