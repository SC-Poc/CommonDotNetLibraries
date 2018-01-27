using JetBrains.Annotations;

namespace Lykke.Common.Chaos
{
    /// <summary>
    /// Settings for the <see cref="IChaosKitty"/>. You can use it right in your app settings
    /// and your can mark the property, that is store <see cref="ChaosSettings"/> as [Optional],
    /// to make it disablable. 
    /// </summary>
    [PublicAPI]
    public class ChaosSettings
    {
        public double StateOfChaos { get; set; }
    }
}