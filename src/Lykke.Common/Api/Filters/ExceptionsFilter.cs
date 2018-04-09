using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Common.Api.Filters
{
    /// <inheritdoc />
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly Dictionary<Type, int> _exceptionMappings;

        /// <summary>
        /// Creates a filter, that allows to respond with specific status code in vase of exception.
        /// </summary>
        public ExceptionFilter(params (Type ExceptionType, int StatusCode)[] exceptionMappings)
        {
            _exceptionMappings = new Dictionary<Type, int>(exceptionMappings.Length);

            foreach ((var type, var statusCode) in exceptionMappings)
            {
                if (!type.IsSubclassOf(typeof(Exception)))
                {
                    throw new ArgumentException
                    (
                        nameof(exceptionMappings),
                        $"Specified type [{type.Name}] is not an exception."
                    );
                }

                _exceptionMappings[type] = statusCode;
            }
        }


        public void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();

            if (_exceptionMappings.TryGetValue(exceptionType, out var statusCode))
            {
                context.HttpContext.Response.StatusCode = statusCode;

                if (statusCode != StatusCodes.Status204NoContent)
                {
                    var responseMessage = ErrorResponse.Create(context.Exception.Message);
                    var serializedResponseMessage = Newtonsoft.Json.JsonConvert.SerializeObject(responseMessage);
                    var responseData = Encoding.UTF8.GetBytes(serializedResponseMessage);

                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.Body.Write(responseData, 0, responseData.Length);
                }

                context.ExceptionHandled = true;
            }
        }
    }
}
