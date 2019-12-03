using System;

namespace Type.BigInteger
{
    public class BigInteger
    {
        public long[] Clusters { get; }
        public bool IsNegative { get; }
        public long Size { get; }

        public BigInteger(string bigInteger)
        {
            if (bigInteger[0] == '-')
            {
                IsNegative = true;
                bigInteger = bigInteger.Substring(1);
            }

            Size = bigInteger.Length;
            Clusters = new long[bigInteger.Length / 18 + 1];
            var zeroes = 18 - bigInteger.Length % 18;
            bigInteger = new string('0', zeroes) + bigInteger;

            var numberIndex = 0;
            for (var i = bigInteger.Length - 18; i >= 0; i -= 18)
            {
                Clusters[numberIndex] = long.Parse(bigInteger.Substring(i, 18));
                numberIndex++;
            }
        }

        public BigInteger Add(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public BigInteger Subtract(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public BigInteger Multiply(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public BigInteger Divide(BigInteger other)
        {
            throw new NotImplementedException();
        }
    }

}
