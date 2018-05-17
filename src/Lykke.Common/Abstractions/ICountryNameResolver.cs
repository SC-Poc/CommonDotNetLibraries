namespace Lykke.Common.Abstractions
{
    /// <summary>
    /// Interface for resolving country name.
    /// </summary>
    public interface ICountryNameResolver
    {
        /// <summary>
        /// Get country name by Iso3 or Iso2 country <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Three or two capital letter country code.</param>
        /// <returns>
        /// <para>Country name for given code.</para>
        /// <para>Empty string if code is null, empty or invalid.</para>
        /// </returns>
        /// <example>
        /// <code>
        /// GetFullNameByCode("RUS");
        /// GetFullNameByCode("RU");
        /// </code>
        /// </example>
        string GetFullNameByCode(string code);
    }
}
