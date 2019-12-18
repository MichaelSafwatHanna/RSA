using System;
using Type.BigInteger;

namespace Crypto.RSA
{
    public static class RSA
    {
        public static BigInteger Encrypt(BigInteger n, BigInteger e, BigInteger m)
        {
            return m.ModOfPower(e, n);
        }

        public static BigInteger Decrypt(BigInteger n, BigInteger d, BigInteger m)
        {
            return m.ModOfPower(d, n);
        }

        public static class KeyGenerator
        {
            private const int MinE = 3;
            private const int MaxE = 65537;
            private static long Gcd(long a, long b, out long x, out long y)
            {
                if (a == 0)
                {
                    x = 0;
                    y = 1;
                    return b;
                }

                var gcd = Gcd(b % a, a, out var x1, out var y1);

                x = y1 - (b / a) * x1;
                y = x1;

                return gcd;
            }

            private static bool IsPrime(int num)
            {
                if (num < 2 || num % 2 == 0) return num == 2;
                var s = num - 1;
                while (s % 2 == 0) s >>= 1;
                var random = new Random();
                for (var i = 0; i < 2; i++)
                {
                    var a = random.Next(num - 1) + 1;
                    var temp = s;
                    long mod = 1;
                    for (var j = 0; j < temp; ++j) mod = mod * a % num;
                    while (temp != num - 1 && mod != 1 && mod != num - 1)
                    {
                        mod = mod * mod % num;
                        temp *= 2;
                    }
                    if (mod != num - 1 && temp % 2 == 0) return false;
                }
                return true;
            }

            private static int GeneratePrime()
            {
                var random = new Random();
                var numOfBits = random.Next(3, 25);
                var loopCount = 100 * (Math.Log(numOfBits, 2) + 1);
                while (loopCount > 0)
                {
                    var n = random.Next((int)Math.Pow(2, numOfBits - 1), (int)Math.Pow(2, numOfBits));
                    loopCount -= 1;
                    if (IsPrime(n)) return n;
                }
                return -1;
            }

            private static long GeneratePublicKey(long phi, long n)
            {
                var loopCount = Math.Min(phi, MaxE);
                var e = MinE;
                for (; e < loopCount; e++)
                {
                    if (Gcd(e, phi, out _, out _) == 1
                        && Gcd(e, n, out _, out _) == 1)
                        return e;
                }

                return e;
            }

            private static long GeneratePrivateKey(long e, long phi)
            {
                var g = Gcd(e, phi, out var x, out _);
                return (g != 1) ? -1 : (x % phi + phi) % phi;
            }

            public static (long, long, long) Generate()
            {
                var p = GeneratePrime();
                var q = GeneratePrime();
                while (p == q)
                    q = GeneratePrime();

                var n = (long)p * q;
                var phi = (long)(p - 1) * (q - 1);

                var e = GeneratePublicKey(phi, n);
                var d = GeneratePrivateKey(e, phi);
                return (n, e, d);
            }

        }

    }

}
