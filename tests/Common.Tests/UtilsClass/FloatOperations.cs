using System.Globalization;
using Xunit;

namespace Common.Tests.UtilsClass
{
    public class FloatOperations
    {
        [Fact]
        public void GetFixedAsStringDecimal()
        {
            var res1 = 0.001m.GetFixedAsString(5);
            var res2 = 15.123456m.GetFixedAsString(2);
            var res3 = 2m.GetFixedAsString(2);
            var res4 = 2m.GetFixedAsString(0);

            Assert.Equal("0.00100", res1);
            Assert.Equal("15.12", res2);
            Assert.Equal("2.00", res3);
            Assert.Equal("2", res4);
        }

        [Fact]
        public void GetFixedAsStringDouble()
        {
            var res1 = 0.001.GetFixedAsString(5);
            var res2 = 15.123456.GetFixedAsString(2);
            var res3 = 2.0.GetFixedAsString(2);
            var res4 = 2.0.GetFixedAsString(0);

            var ct = new TestDouble();
            var res5 = ct.D.GetFixedAsString(2);

            var dnan = double.NaN;
            var res6 = dnan.GetFixedAsString(2);

            Assert.Equal("0.00100", res1);
            Assert.Equal("15.12", res2);
            Assert.Equal("2.00", res3);
            Assert.Equal("2", res4);
            Assert.Equal("0.00", res5);
            Assert.Equal("NaN", res6);
        }

        public class TestDouble
        {
            public double D { get; set; }
        }
    }
}
