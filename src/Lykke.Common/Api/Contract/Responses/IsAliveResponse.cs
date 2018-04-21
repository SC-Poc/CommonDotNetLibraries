using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Common.Api.Contract.Responses
{
    /// <summary>
    /// General API IsAlive response.
    /// See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    /// </summary>
    [PublicAPI]
    public class IsAliveResponse
    {
        /// <summary>
        /// App name
        /// </summary>
        [CanBeNull]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// App version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// ENV_INFO environment variable value
        /// </summary>
        [JsonProperty("env")]
        public string Env { get; set; }

        /// <summary>
        /// Flag which indicates, is app build DEBUG or RELEASE
        /// </summary>
        [JsonProperty("isDebug")]
        public bool IsDebug { get; set; }

        /// <summary>
        /// App issue indicators
        /// </summary>
        [JsonProperty("issueIndicators")]
        public IEnumerable<IssueIndicator> IssueIndicators { get; set; }

        /// <summary>
        /// App issue indicator
        /// </summary>
        [PublicAPI]
        public class IssueIndicator
        {
            /// <summary>
            /// Indicator type
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }

            /// <summary>
            /// Indicator value
            /// </summary>
            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
