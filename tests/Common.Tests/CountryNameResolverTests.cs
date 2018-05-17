using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Common;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests
{
    public class CountryNameResolverTests
    {

        [Fact]
        public void GetFullNameByCode_Should_HaveNamesForAllCodes()
        {
            var countryNameResolver = new CountryNameResolver();

            foreach (var codePair in CountryManager.CountryIso3ToIso2Links)
            {
                var iso3Code = codePair.Key;
                var iso2Code = codePair.Value;

                var countryNameByIso3 = countryNameResolver.GetFullNameByCode(iso3Code);
                var countryNameByIso2 = countryNameResolver.GetFullNameByCode(iso2Code);

                Assert.Equal(countryNameByIso3, countryNameByIso2);
                Assert.NotEqual(countryNameByIso3, string.Empty);
            }
        }

        [Fact]
        public void GetFullNameByCode_For_InvalidCode_Should_ReturnEmptyString()
        {
            var countryNameResolver = new CountryNameResolver();

            const string invalidIso = "InvalidIso";

            var countryNameByIso3 = countryNameResolver.GetFullNameByCode(invalidIso);
            var countryNameByIso2 = countryNameResolver.GetFullNameByCode(invalidIso);

            Assert.Equal(countryNameByIso3, string.Empty);
            Assert.Equal(countryNameByIso2, string.Empty);
        }

        [Fact]
        public void GetFullNameByCode_For_Null_Should_ReturnEmptyString()
        {
            var countryNameResolver = new CountryNameResolver();
            var countryName = countryNameResolver.GetFullNameByCode(null);
            Assert.Equal(countryName, string.Empty);
        }

    }
}
