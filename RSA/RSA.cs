using Type.BigInteger;

namespace Crypto.RSA
{
    static public class RSA
    {
        static public BigInteger Encrypt(BigInteger n, BigInteger e, BigInteger m)
        {
            return m.ModOfPower(e, n);
        }

        static public BigInteger Decrypt(BigInteger n, BigInteger d, BigInteger m)
        {
            return m.ModOfPower(d, n);
        }
    }
}
