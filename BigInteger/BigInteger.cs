using System;

namespace Type.BigInteger
{
    public class BigInteger
    {
        const int CLUSTER_SIZE = 18;
        const long MAX_CLUSTER_VALUE = 1000000000000000000;
        public ulong[] Clusters { get; }
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
            int quotient = bigInteger.Length / CLUSTER_SIZE;
            int remainder = bigInteger.Length % CLUSTER_SIZE;
            Clusters = new ulong[quotient + (remainder == 0 ? 0 : 1)];

            var numberIndex = 0;
            for (var i = bigInteger.Length - CLUSTER_SIZE; i >= 0; i -= CLUSTER_SIZE)
            {
                Clusters[numberIndex] = ulong.Parse(bigInteger.Substring(i, CLUSTER_SIZE));
                numberIndex++;
            }
            if (remainder != 0)
            {
                Clusters[numberIndex] = ulong.Parse(bigInteger.Substring(0, remainder));
            }
        }

        public BigInteger Add(BigInteger other)
        {
            string result = "";
            BigInteger first = this;
            BigInteger second = other;
            if (Size < other.Size)
            {
                first = other;
                second = this;
            }

            ulong carry = 0;
            for (int i = 0; i < second.Clusters.Length; i++)
            {
                ulong sum = first.Clusters[i] + second.Clusters[i] + carry;
                carry = 0;
                if (sum >= MAX_CLUSTER_VALUE)
                {
                    carry = 1;
                    sum %= MAX_CLUSTER_VALUE;
                }
                
                result = sum.ToString().PadLeft(CLUSTER_SIZE, '0') + result;
            }

            for (int i = second.Clusters.Length; i < first.Clusters.Length; i++)
            {
                ulong sum = first.Clusters[i] + carry;
                carry = 0;
                if (sum >= MAX_CLUSTER_VALUE)
                {
                    carry = 1;
                    sum %= MAX_CLUSTER_VALUE;
                }
                result = sum.ToString().PadLeft(CLUSTER_SIZE, '0') + result;
            }

            if (carry == 1)
                result = "1" + result;

            return new BigInteger(result);

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

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Clusters.Length - 1; i++)
            {
                s = Clusters[i].ToString().PadLeft(CLUSTER_SIZE, '0') + s;
            }

            return Clusters[Clusters.Length - 1].ToString() + s;
        }
    }

}
