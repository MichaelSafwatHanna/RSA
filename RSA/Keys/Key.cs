using Type.BigInteger;

namespace Crypto.RSA.Keys
{
    public abstract class Key
    {
        public BigInteger Exponent { get; }
        public BigInteger Modulus { get; }

        protected Key(BigInteger exponent, BigInteger modulus)
        {
            Exponent = exponent;
            Modulus = modulus;
        }
    }
}
