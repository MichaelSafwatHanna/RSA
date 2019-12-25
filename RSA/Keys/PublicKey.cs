using Type.BigInteger;

namespace Crypto.RSA.Keys
{
    public class PublicKey : Key
    {
        public PublicKey(BigInteger exponent, BigInteger modulus) : base(exponent, modulus)
        {
        }
    }
}