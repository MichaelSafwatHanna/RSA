using Crypto.RSA.Encoders;
using Crypto.RSA.Keys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Type.BigInteger;

namespace Crypto.RSA.Tests
{
    [TestClass]
    public class RSATests
    {
        [DataTestMethod]
        [DynamicData(nameof(DataSet.Encrypt), typeof(DataSet), DynamicDataSourceType.Method)]
        public void EncryptTest(string n, string e, string m, string output)
        {
            var key = new PublicKey(new BigInteger(e), new BigInteger(n));
            var message = new BigInteger(m);
            var actual = RSA.Encrypt(key, message);
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Decrypt), typeof(DataSet), DynamicDataSourceType.Method)]
        public void DecryptTest(string n, string d, string em, string output)
        {
            var key = new PrivateKey(new BigInteger(d), new BigInteger(n));
            var encryptedMessage = new BigInteger(em);
            var actual = RSA.Decrypt(key, encryptedMessage);
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.EncryptAscii), typeof(DataSet), DynamicDataSourceType.Method)]
        public void EncryptAsciiTest(string n, string e, string m, string output)
        {
            var key = new PublicKey(new BigInteger(e), new BigInteger(n));
            var message = m;
            var actual = RSA.Encrypt(key, message, AsciiEncoder.Instance);
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.DecryptAscii), typeof(DataSet), DynamicDataSourceType.Method)]
        public void DecryptAsciiTest(string n, string d, string em, string output)
        {
            var key = new PrivateKey(new BigInteger(d), new BigInteger(n));
            var encryptedMessage = new BigInteger(em);
            var actual = RSA.Decrypt(key, encryptedMessage, AsciiEncoder.Instance);
            var expected = output;
            Assert.AreEqual(expected, actual);
        }
    }
}
