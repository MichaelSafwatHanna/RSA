using System;

namespace Type.BigInteger
{
    public class BigInteger
    {
        private const int ClusterSize = 18;
        private const long MaxClusterValue = (long)1E18;

        public bool IsNegative { get; }
        public int Size { get; set; }
        public int ClustersLength { get; set; }
        public ulong[] Clusters { get; }

        public BigInteger(string input)
        {
            IsNegative = input[0] == '-';
            var offset = IsNegative ? 1 : 0;

            Size = input.Length - offset;
            ClustersLength = (int)Math.Ceiling((double)Size / ClusterSize);
            Clusters = new ulong[ClustersLength];

            var clusterIndex = 0;
            for (var i = input.Length - ClusterSize - offset; i >= offset; i -= ClusterSize)
            {
                Clusters[clusterIndex] = Convert.ToUInt64(input.Substring(i + offset, ClusterSize));
                clusterIndex++;
            }

            var remainder = Size % ClusterSize;
            if (remainder != 0) Clusters[clusterIndex] = Convert.ToUInt64(input.Substring(offset, remainder));
        }

        public BigInteger(int clustersLength)
        {
            Size = clustersLength * ClusterSize;
            ClustersLength = clustersLength;
            Clusters = new ulong[ClustersLength];
        }

        public BigInteger Add(BigInteger other)
        {
            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length + 1);
            var carry = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i < ClustersLength ? Clusters[i] : 0;
                var operand2 = i < other.ClustersLength ? other.Clusters[i] : 0;
                var sum = operand1 + operand2 + (ulong)carry;
                carry = sum >= MaxClusterValue ? 1 : 0;
                result.Clusters[i] = sum % MaxClusterValue;
            }

            if (carry == 1)
            {
                result.Clusters[result.ClustersLength - 1] = (ulong)carry;
                result.Size -= ClusterSize - 1;
            }
            else
            {
                result.ClustersLength--;
                var lastClusterLength = result.Clusters[result.ClustersLength - 1].ToString().Length;
                result.Size -= 2 * ClusterSize - lastClusterLength;
            }

            return result;
        }

        public BigInteger Subtract(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public BigInteger Multiply(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public void Divide(BigInteger other, out BigInteger quotient, out BigInteger remainder)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var str = "";
            for (var i = 0; i < ClustersLength - 1; i++)
            {
                str = Clusters[i].ToString().PadLeft(ClusterSize, '0') + str;
            }

            return (IsNegative ? "-" : "") + Clusters[ClustersLength - 1] + str;
        }

        private bool Equals(BigInteger other)
        {
            if (ClustersLength != other.ClustersLength) return false;
            for (var i = 0; i < ClustersLength; i++)
            {
                if (Clusters[i] != other.Clusters[i]) return false;
            }
            return IsNegative == other.IsNegative && Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((BigInteger)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsNegative.GetHashCode();
                hashCode = (hashCode * 397) ^ (Clusters != null ? Clusters.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

}
