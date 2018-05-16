using System;
using JetBrains.Annotations;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Common
{
    /// <summary>
    /// Executing application environment
    /// </summary>
    [PublicAPI]
    public static class AppEnvironment
    {
        /// <summary>
        /// ENV_INFO environment variable 
        /// </summary>
        public static string EnvInfo { get; } = Environment.GetEnvironmentVariable("ENV_INFO");

        /// <summary>
        /// Version of the app
        /// </summary>
        public static string Version { get; } = PlatformServices.Default.Application.ApplicationVersion;

        /// <summary>
        /// Name of the app
        /// </summary>
        public static string Name { get; } = PlatformServices.Default.Application.ApplicationName;
    }
}
