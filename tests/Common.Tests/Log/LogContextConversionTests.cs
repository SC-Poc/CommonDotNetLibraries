using Lykke.Common.Log;
using Xunit;

namespace Common.Tests.Log
{
    public class LogContextConversionTests
    {
        [Fact]
        public void Conversion_to_string_works_correctly()
        {
            // Act

            var str1 = LogContextConversion.ConvertToString("context1");
            var str2 = LogContextConversion.ConvertToString(new
            {
                text = "str",
                number = 1
            });
            var str3 = LogContextConversion.ConvertToString(null);

            // Assert

            Assert.Equal("context1", str1);

            const string expectedContext2 = "{\r\n  \"text\": \"str\",\r\n  \"number\": 1\r\n}";

            Assert.Equal(expectedContext2, str2);
            Assert.Null(str3);
        }
    }
}
