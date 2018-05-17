using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Lykke.Common.Abstractions;

namespace Lykke.Common
{
    /// <inheritdoc />
    /// <summary>
    /// Class for resolving country name.
    /// </summary>
    public class CountryNameResolver: ICountryNameResolver
    {
        private const int Iso3CodeLength = 3;

        /// <inheritdoc />
        public string GetFullNameByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;
            return code.Length == Iso3CodeLength ? CountryManager.GetCountryNameByIso3(code) : CountryManager.GetCountryNameByIso2(code);
        }
    }
}
