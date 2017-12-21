using Xunit;

namespace Common.Tests.UtilsClass
{
    public class StringUtilsTests
    {
        [Fact]
        public void SanitizePhone()
        {
            var phone = "+71234567890";
            var sanitized = phone.SanitizePhone();

            Assert.Equal("+7123*******", sanitized);

            phone = "1234";
            sanitized = phone.SanitizePhone();
            
            Assert.Equal("1234", sanitized);
            
            phone = "+12345";
            sanitized = phone.SanitizePhone();
            
            Assert.Equal("+1234*", sanitized);
            
            phone = "";
            sanitized = phone.SanitizePhone();
            
            Assert.Equal("", sanitized);
        }
    }
}
