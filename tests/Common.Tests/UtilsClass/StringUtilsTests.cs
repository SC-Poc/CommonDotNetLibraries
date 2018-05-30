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
        public void Test_IsPasswordComplex_Valid()
        {
            string password = "Qwer12$_@#";
            Assert.True(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_With_Unicode_Char()
        {
            string password = "Qwer!@122┴";
            Assert.True(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_With_NonEnglish_Char()
        {
            string password = "QwerÄ1223┴";
            Assert.True(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_With_NoEnglish_Char()
        {
            string password = "сложныйПароль123#!@";
            Assert.True(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_Disabled_Special_Chars()
        {
            string password = "Qwer123456";
            Assert.True(password.IsPasswordComplex(useSpecialChars:false));
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_Disabled_Special_Chars_And_Changed_MinLength()
        {
            string password = "Qwer123456";
            Assert.True(password.IsPasswordComplex(4, useSpecialChars:false));
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_No_Chars_Sequence()
        {
            string password = "Qwer!1223456";
            Assert.True(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_Increased_Chars_Sequence()
        {
            string password = "Qwer!12223456";
            Assert.True(password.IsPasswordComplex(charsSequence:4));
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Valid_Disabled_Chars_Sequence()
        {
            string password = "Qwer!11111";
            Assert.True(password.IsPasswordComplex(useCharsSequence:false));
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Special_Chars()
        {
            string password = "Qwer123456";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_Length()
        {
            string password = "Qwe123$";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Digits()
        {
            string password = "Qwe#@!!_$";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Upper_Case()
        {
            string password = "qwe1#@!!_$";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Lower_Case()
        {
            string password = "QWE1#@!!_$";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Upper_Case_NonEnglish()
        {
            string password = "сложный123#!@";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_No_Lower_Case_NonEnglish()
        {
            string password = "СЛОЖНЫЙ123#!@";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_Char_Sequence()
        {
            string password = "Qwer!12223456";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_Special_Char_Sequence()
        {
            string password = "Qwer!!!!1234";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_NonEnglish_Char_Sequence()
        {
            string password = "QwerÄÄÄ1234";
            Assert.False(password.IsPasswordComplex());
        }
        
        [Fact]
        public void Test_IsPasswordComplex_Invalid_Unicode_Char_Sequence()
        {
            string password = "Qwer┴┴┴1234";
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
