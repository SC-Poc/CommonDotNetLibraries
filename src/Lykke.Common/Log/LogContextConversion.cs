using System;
using AsyncFriendlyStackTrace;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Common.Log
{
    /// <summary>
    /// Conversion of the log entry context
    /// </summary>
    [PublicAPI]
    public static class LogContextConversion
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        static LogContextConversion()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <summary>
        /// Converts log entry context to the string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [CanBeNull]
        public static string ConvertToString([CanBeNull] object context)
        {
            switch (context)
            {
                case null:
                    return null;

                case string str:
                    return str;
            }

            try
            {
                return JsonConvert.SerializeObject(context, SerializerSettings);
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine(ex.ToAsyncString());
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            return null;
        }
    }
}
