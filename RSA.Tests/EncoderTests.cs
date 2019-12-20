using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crypto.RSA.Tests
{
    [TestClass]
    public class EncoderTests
    {
        [DataTestMethod]
        [DynamicData(nameof(DataSet.Encoder), typeof(DataSet), DynamicDataSourceType.Method)]
        public void GetBytesTest(string str, string bytes)
        {
            var actual = RSA.Encoder.GetBytes(str);
            Assert.AreEqual(bytes, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Encoder), typeof(DataSet), DynamicDataSourceType.Method)]
        public void GetStringTest(string str, string bytes)
        {
            var actual = RSA.Encoder.GetString(bytes);
            Assert.AreEqual(str, actual);
        }
    }
}
