using Xunit;

namespace Common.Tests
{
    public class CountryManagerTests
    {
        [Fact]
        public void HasIso2_Should_Return_False_When_Key_Is_Null()
        {
            Assert.False(CountryManager.HasIso2(null));
        }

        [Fact]
        public void HasIso3_Should_Return_False_When_Key_Is_Null()
        {
            Assert.False(CountryManager.HasIso3(null));
        }

        [Fact]
        public void Iso3ToIso2_Should_Return_Empty_String_When_Key_Is_Null()
        {
            Assert.Empty(CountryManager.Iso3ToIso2(null));
        }

        [Fact]
        public void Iso2ToIso3_Should_Return_Empty_String_When_Key_Is_Null()
        {
            Assert.Empty(CountryManager.Iso2ToIso3(null));
        }

        [Fact]
        public void GetCountryNameByIso3_Should_Return_Empty_String_When_Key_Is_Null()
        {
            Assert.Empty(CountryManager.GetCountryNameByIso3(null));
        }

        [Fact]
        public void GetCountryNameByIso2_Should_Return_Empty_String_When_Key_Is_Null()
        {
            Assert.Empty(CountryManager.GetCountryNameByIso2(null));
        }
    }
}
