using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Lykke.Common.Api.Handlers
{
    [PublicAPI]
    public class HttpErrorLoggingHandler : DelegatingHandler
    {
        private readonly ILog _log;
        private readonly List<(string Pattern, string Replacement)> _sensitivePatterns;

        [Obsolete("Use public HttpErrorLoggingHandler(ILogFactory logFactory, HttpMessageHandler innerHandler = null)")]
        public HttpErrorLoggingHandler(ILog log, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _log = log;
            _sensitivePatterns = new List<(string Pattern, string Replacement)>();
        }

        public HttpErrorLoggingHandler(ILogFactory logFactory, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _log = logFactory.CreateLog(this);
            _sensitivePatterns = new List<(string Pattern, string Replacement)>();
        }

        /// <summary>
        ///    Adds sensitive pattern that should not be logged. Api keys, private keys and so on, for example.
        /// </summary>
        /// <param name="pattern">
        ///    Regex that should be replaced.
        /// </param>
        /// <param name="replacement">
        ///    Pattern replacement.
        /// </param>
        public void AddSensitivePattern(string pattern, string replacement)
        {
            _sensitivePatterns.Add((pattern, replacement));
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await LogRequestAsync(request, id);
                await LogResponseAsync(response, id);
            }

            return response;
        }

        private static bool IsTextBasedContentType(HttpHeaders headers)
        {
            string[] types = {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};

            if (!headers.TryGetValues("Content-Type", out var values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();

            return types.Any(t => header.Contains(t));
        }

        private async Task LogRequestAsync(HttpRequestMessage request, Guid id)
        {
            var message = new StringBuilder();

            message.AppendLine($"Request {id}: {request.Method.ToString().ToUpper()} {request.RequestUri}");

            foreach (var header in request.Headers)
            {
                message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                if (request.Content is StringContent ||
                    IsTextBasedContentType(request.Headers) ||
                    IsTextBasedContentType(request.Content.Headers))
                {
                    var content = await request.Content.ReadAsStringAsync();

                    message.AppendLine(content);
                }
            }

            _log.WriteWarning("HTTP API request ->", CleanupSensitiveInformation(message.ToString()), "Response status is non successful");
        }

        private async Task LogResponseAsync(HttpResponseMessage response, Guid id)
        {
            var message = new StringBuilder();

            message.AppendLine($"Response {id}: {(int) response.StatusCode} {response.StatusCode} - {response.ReasonPhrase}");

            foreach (var header in response.Headers)
            {
                message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                if (response.Content is StringContent ||
                    IsTextBasedContentType(response.Headers) ||
                    IsTextBasedContentType(response.Content.Headers))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    message.AppendLine(content);
                }
            }

            _log.WriteWarning("HTTP API response <-", message.ToString(), "Response status is non successful");
        }
        
        private string CleanupSensitiveInformation(string message)
        {
            const RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

            return _sensitivePatterns.Aggregate
            (
                message,
                (current, sensitivePattern) => Regex.Replace(current, sensitivePattern.Pattern, sensitivePattern.Replacement, options)
            );
        }
    }
}
