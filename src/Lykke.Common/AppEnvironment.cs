using System;
using System.Reflection;
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
        public static string Version { get; } = Assembly.GetEntryAssembly().GetName().Version.ToString();

        /// <summary>
        /// Name of the app
        /// </summary>
        public static string Name { get; } = Assembly.GetEntryAssembly().GetName().Name;
    }
}
