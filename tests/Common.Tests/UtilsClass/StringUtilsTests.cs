﻿using System;
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
            const int persistedHash = 1778674507;

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

            Assert.Equal("B", hash1);
            Assert.Equal("6A046B4B", hash8);
        }
    }
}