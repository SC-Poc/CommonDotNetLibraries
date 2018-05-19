using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Common.Log
{
    /// <summary>
    /// Conversion of the log entry context
    /// </summary>
    internal static class LogContextConversion
    {
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

            return JsonConvert.SerializeObject(context, Formatting.Indented);
        }
    }
}
