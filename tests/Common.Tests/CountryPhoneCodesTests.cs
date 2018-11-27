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
    }
}
