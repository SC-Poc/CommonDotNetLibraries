using System;
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

        [Fact]
        public void Test_that_hash64_can_be_persisted()
        {
            const string str = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";
            const long persistedHash = 5363730202200970195;

            var hash = str.CalculateHash64();

            Assert.Equal(persistedHash, hash);
        }

        [Fact]
        public void Test_that_hash32_can_be_persisted()
        {
            const string str = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";
            const int persistedHash = 1749566419;

            var hash = str.CalculateHash32();

            Assert.Equal(persistedHash, hash);
        }

        [Fact]
        public void Test_hex_hash64()
        {
            const string str = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";

            Assert.Throws<ArgumentOutOfRangeException>(() => str.CalculateHexHash64(0));
            var hash1 = str.CalculateHexHash64(1);
            var hash8 = str.CalculateHexHash64(8);
            var hash16 = str.CalculateHexHash64(16);
            Assert.Throws<ArgumentOutOfRangeException>(() => str.CalculateHexHash64(17));

            Assert.Equal("3", hash1);
            Assert.Equal("5D3913D3", hash8);
            Assert.Equal("4A6FCC335D3913D3", hash16);
        }

        [Fact]
        public void Test_hex_hash32()
        {
            const string str = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";

            Assert.Throws<ArgumentOutOfRangeException>(() => str.CalculateHexHash32(0));
            var hash1 = str.CalculateHexHash32(1);
            var hash8 = str.CalculateHexHash32(8);
            Assert.Throws<ArgumentOutOfRangeException>(() => str.CalculateHexHash32(9));

            Assert.Equal("3", hash1);
            Assert.Equal("684843D3", hash8);
        }
        
        [Fact]
        public void Test_IsPasswordComplex()
        {
            //complex password: >=8 chars, lower, upper and special chars
            string password = "Qwer12$_@#";
            Assert.True(password.IsPasswordComplex());
            
            //complex password: unicode char
            password = "Qwer122┴";
            Assert.True(password.IsPasswordComplex());
            
            //complex password: non english chars
            password = "QwerÄ222┴";
            Assert.True(password.IsPasswordComplex());
            
            //complex password: non english chars
            password = "сложныйПароль123#!@";
            Assert.True(password.IsPasswordComplex());
            
            //complex password: don't use special chars
            password = "Qwer1234";
            Assert.True(password.IsPasswordComplex(useSpecialChars:false));
            
            //complex password: don't use special chars and min length = 4 chars
            password = "Qwer1234";
            Assert.True(password.IsPasswordComplex(4, false));
            
            //not complex password: no special chars
            password = "Qwer1234";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: < 8 chars
            password = "Qwe123$";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: no digits
            password = "Qwe#@!!_$";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: no upper case chars
            password = "qwe1#@!!_$";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: no lower case chars
            password = "QWE1#@!!_$";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: no upper case chars (non english)
            password = "сложный123#!@";
            Assert.False(password.IsPasswordComplex());
            
            //not complex password: no upper case chars (non english)
            password = "СЛОЖНЫЙ123#!@";
            Assert.False(password.IsPasswordComplex());
        }

        [Fact]
        public void Test_SanitizeIp()
        {
            string ip = null;
            Assert.Equal(string.Empty, ip.SanitizeIp());
            
            ip = string.Empty;
            Assert.Equal(string.Empty, ip.SanitizeIp());
            
            ip = "    ";
            Assert.Equal(string.Empty, ip.SanitizeIp());
            
            ip = "::1";
            Assert.Equal("::1", ip.SanitizeIp());
            
            ip = "127.0";
            Assert.Equal("127.0", ip.SanitizeIp());
            
            ip = "192.168.1.100";
            Assert.Equal("192.168.1.0", ip.SanitizeIp());
        }
    }
}
