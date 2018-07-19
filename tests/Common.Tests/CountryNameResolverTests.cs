using Lykke.Common;
using Xunit;

namespace Common.Tests
{
    public class CountryNameResolverTests
    {

        [Fact]
        public void GetFullNameByCode_Should_HaveNamesForAllCodes()
        {
            // Arrange
            var countryNameResolver = new CountryNameResolver();

            foreach (var codePair in CountryManager.CountryIso3ToIso2Links)
            {
                var iso3Code = codePair.Key;
                var iso2Code = codePair.Value;

                // Act
                var countryNameByIso3 = countryNameResolver.GetFullNameByCode(iso3Code);
                var countryNameByIso2 = countryNameResolver.GetFullNameByCode(iso2Code);

                // Assert
                Assert.Equal(countryNameByIso3, countryNameByIso2);
                Assert.NotEqual(countryNameByIso3, string.Empty);
            }
        }

        [Fact]
        public void GetFullNameByCode_InvalidCode_ReturnEmptyString()
        {
            // Arrange
            var countryNameResolver = new CountryNameResolver();

            const string invalidIso = "InvalidIso";

            // Act
            var countryNameByIso3 = countryNameResolver.GetFullNameByCode(invalidIso);
            var countryNameByIso2 = countryNameResolver.GetFullNameByCode(invalidIso);

            // Assert
            Assert.Equal(countryNameByIso3, string.Empty);
            Assert.Equal(countryNameByIso2, string.Empty);
        }

        [Fact]
        public void GetFullNameByCode_Null_ReturnEmptyString()
        {
            // Arrange
            var countryNameResolver = new CountryNameResolver();

            // Act
            var countryName = countryNameResolver.GetFullNameByCode(null);

            // Assert
            Assert.Equal(countryName, string.Empty);
        }
    }
}
