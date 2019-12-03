using System;

namespace RSA
{
    public class BigInteger
    {
        long[] Clusters { get; }
        bool IsNegative { get; }
        long Size { get; }

        public BigInteger(string bigInteger)
        {
            if (bigInteger[0] == '-')
            {
                IsNegative = true;
                bigInteger = bigInteger.Substring(1);
            }

            Size = bigInteger.Length;
            Clusters = new long[(bigInteger.Length / 18) + 1];
            int zeroes = 18 - (bigInteger.Length % 18);
            bigInteger = new string('0', zeroes) + bigInteger;

            int numberIndex = 0;
            for (int i = bigInteger.Length - 18; i >= 0; i -= 18)
            {
                Clusters[numberIndex] = long.Parse(bigInteger.Substring(i, 18));
                numberIndex++;
            }
        }

        BigInteger Add(BigInteger other)
        {
            throw new NotImplementedException();
        }

        BigInteger Subtract(BigInteger other)
        {
            throw new NotImplementedException();
        }

        BigInteger Mutiply(BigInteger other)
        {
            throw new NotImplementedException();
        }

        BigInteger Divide(BigInteger other)
        {
            throw new NotImplementedException();
        }
    }
}
