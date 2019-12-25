using Type.BigInteger;

namespace Crypto.RSA.Keys
{
    public class PrivateKey : Key
    {
        public PrivateKey(BigInteger exponent, BigInteger modulus) : base(exponent, modulus)
        {
        }
    }
}