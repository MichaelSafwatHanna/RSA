using Crypto.RSA.Encoders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crypto.RSA.Tests
{
    [TestClass]
    public class AsciiEncoderTests
    {
        [DataTestMethod]
        [DynamicData(nameof(DataSet.AsciiEncoder), typeof(DataSet), DynamicDataSourceType.Method)]
        public void GetBytesTest(string str, string bytes)
        {
            var actual = AsciiEncoder.Instance.GetBytes(str);
            Assert.AreEqual(bytes, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.AsciiEncoder), typeof(DataSet), DynamicDataSourceType.Method)]
        public void GetStringTest(string str, string bytes)
        {
            var actual = AsciiEncoder.Instance.GetString(bytes);
            Assert.AreEqual(str, actual);
        }
    }
}
