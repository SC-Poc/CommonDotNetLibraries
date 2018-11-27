using System;
using Common;

namespace Lykke.Common
{
    public class CountryItem
    {
        public string Id { get; }
        public string Iso2 { get; }
        public string Name { get; }
        public string Prefix { get; }

        public CountryItem(string iso3, string prefix)
        {
            if (!CountryManager.HasIso3(iso3))
                throw new ArgumentException($"ISO3 code doesn't exist ({iso3})", nameof(iso3));

            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            Id = iso3;
            Iso2 = CountryManager.Iso3ToIso2(iso3);
            Name = CountryManager.GetCountryNameByIso3(iso3);
            Prefix = prefix;
        }
    }
}
