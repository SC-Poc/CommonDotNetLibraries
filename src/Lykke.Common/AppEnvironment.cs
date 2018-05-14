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
        public static readonly string EnvInfo = Environment.GetEnvironmentVariable("ENV_INFO");
        public static readonly string Version = PlatformServices.Default.Application.ApplicationVersion;
        public static readonly string Name = PlatformServices.Default.Application.ApplicationName;
    }
}
