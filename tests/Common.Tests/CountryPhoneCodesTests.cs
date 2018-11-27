using System;
using System.Linq;
using Lykke.Common;
using Xunit;

namespace Common.Tests
{
    public class CountryPhoneCodesTests
    {
        [Fact]
        public void InitCountries_ReturnsCountryList_NoEmptyCountries()
        {
            // Arrange
            var phoneCodes = new CountryPhoneCodes();

            //Act
            var countries = phoneCodes.GetCountries().ToList();

            //Assert
            Assert.All(countries, x => Assert.False(string.IsNullOrEmpty(x.Iso2)));
            Assert.All(countries, x => Assert.False(string.IsNullOrEmpty(x.Name)));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("whatever")]
        public void NewCountryItem_InvalidIso3Code_RaisesException(string iso3)
        {
            Assert.Throws<ArgumentException>(() => new CountryItem(iso3, "any phone prefix"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void NewCountryItem_InvalidPrefix_RaisesException(string prefix)
        {
            var validIso3 = CountryManager.CountryIso3ToNameLinks.First().Key;

            Assert.Throws<ArgumentNullException>(() => new CountryItem(validIso3, prefix));
        }
    }
}
