﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Lykke.Common.Api.Contract.Responses
{
    /// <summary>
    /// General API error response
    /// </summary>
    [PublicAPI]
    public class ErrorResponse
    {
        /// <summary>
        /// Summary error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Model errors. Key is the model field name, value is the list of the errors related to the given model field.
        /// </summary>
        public Dictionary<string, List<string>> ModelErrors { get; set; }

        /// <summary>
        /// Creates <see cref="ErrorResponse"/> with summary error message
        /// </summary>
        /// <param name="message">Summary error message</param>
        public static ErrorResponse Create(string message)
        {
            return new ErrorResponse
            {
                ErrorMessage = message,
                ModelErrors = new Dictionary<string, List<string>>()
            };
        }

        /// <summary>
        /// Adds model error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">Model field name</param>
        /// <param name="message">Error related to the given model field</param>
        /// <returns></returns>
        public ErrorResponse AddModelError(string key, string message)
        {
            if (ModelErrors == null)
            {
                ModelErrors = new Dictionary<string, List<string>>();
            }

            if (!ModelErrors.TryGetValue(key, out var errors))
            {
                errors = new List<string>();

                ModelErrors.Add(key, errors);
            }

            errors.Add(message);

            return this;
        }

        /// <summary>
        /// Adds model error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">Model field name</param>
        /// <param name="exception">Exception which corresponds to the error related to the given model field</param>
        public ErrorResponse AddModelError(string key, Exception exception)
        {
            var ex = exception;
            var sb = new StringBuilder();

            while (true)
            {
                if (ex.InnerException != null)
                {
                    sb.AppendLine(ex.Message);
                }
                else
                {
                    sb.Append(ex.Message);
                }

                ex = ex.InnerException;

                if (ex == null)
                {
                    return AddModelError(key, sb.ToString());
                }

                sb.Append(" -> ");
            }
        }
    }
}
