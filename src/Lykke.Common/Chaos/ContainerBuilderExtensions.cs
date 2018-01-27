using Autofac;
using JetBrains.Annotations;

namespace Lykke.Common.Chaos
{
    /// <summary>
    /// Autofcat <see cref="ContainerBuilder"/> extensions for the <see cref="ChaosKitty"/>
    /// </summary>
    [PublicAPI]
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers <see cref="ChaosKitty"/> singleton as <see cref="IChaosKitty"/>
        /// </summary>
        /// <param name="builder">The DI container builder</param>
        /// <param name="settings">Chaos settings. pass null to disable chaos</param>
        public static void RegisterChaosKitty([NotNull] this ContainerBuilder builder, [CanBeNull] ChaosSettings settings)
        {
            if (settings != null)
            {
                builder.RegisterType<ChaosKitty>()
                    .As<IChaosKitty>()
                    .WithParameter(TypedParameter.From(settings.StateOfChaos))
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<SilentChaosKitty>()
                    .As<IChaosKitty>();
            }
        }
    }
}
