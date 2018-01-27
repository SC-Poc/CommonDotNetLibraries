namespace Lykke.Common.Chaos
{
    /// <summary>
    /// Chaos kitty, that never meows
    /// </summary>
    public class SilentChaosKitty : IChaosKitty
    {
        /// <inheritdoc />
        public void Meow(object tag, int lineNumber = 0)
        {
        }
    }
}
