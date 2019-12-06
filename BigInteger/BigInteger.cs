using System;

namespace Type.BigInteger
{
    public class BigInteger
    {
        private const int ClusterSize = 18;
        private const long MaxClusterValue = (long)1E18;

        private bool IsNegative { get; }
        public int Size { get; private set; }
        public int ClustersLength { get; private set; }
        private long[] Clusters { get; }

        public BigInteger(string input)
        {
            IsNegative = input[0] == '-';
            var offset = IsNegative ? 1 : 0;

            Size = input.Length - offset;
            ClustersLength = (int)Math.Ceiling((double)Size / ClusterSize);
            Clusters = new long[ClustersLength];

            var clusterIndex = 0;
            for (var i = input.Length - ClusterSize - offset; i >= offset; i -= ClusterSize)
            {
                Clusters[clusterIndex] = Convert.ToInt64(input.Substring(i + offset, ClusterSize));
                clusterIndex++;
            }

            var remainder = Size % ClusterSize;
            if (remainder != 0) Clusters[clusterIndex] = Convert.ToInt64(input.Substring(offset, remainder));
        }

        private BigInteger(int clustersLength)
        {
            Size = clustersLength * ClusterSize;
            ClustersLength = clustersLength;
            Clusters = new long[ClustersLength];
        }

        private void RemoveLastCluster()
        {
            ClustersLength--;
            Size -= ClusterSize;
        }

        public BigInteger Add(BigInteger other)
        {
            if (other.IsNegative) return Subtract(other);

            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length + 1);
            var carry = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i < ClustersLength ? Clusters[i] : 0;
                var operand2 = i < other.ClustersLength ? other.Clusters[i] : 0;
                var sum = operand1 + operand2 + carry;
                carry = sum >= MaxClusterValue ? 1 : 0;
                result.Clusters[i] = sum % MaxClusterValue;
            }

            result.Clusters[result.ClustersLength - 1] = carry;

            // Adjust Size
            if (carry == 0) result.RemoveLastCluster();
            var lastClusterLength = result.Clusters[result.ClustersLength - 1].ToString().Length;
            result.Size -= ClusterSize - lastClusterLength;

            return result;
        }

        public BigInteger Subtract(BigInteger other)
        {
            if (other.IsNegative) return Add(other);

            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length);
            var borrow = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i < ClustersLength ? Clusters[i] : 0;
                var operand2 = i < other.ClustersLength ? other.Clusters[i] : 0;
                var diff = operand1 - operand2 - borrow;
                borrow = diff < 0 ? 1 : 0;
                result.Clusters[i] = diff + borrow * MaxClusterValue;
            }

            // Remove Empty Clusters
            while (result.ClustersLength > 1 && result.Clusters[result.ClustersLength - 1] == 0)
            {
                result.RemoveLastCluster();
            }

            // Adjust Size
            var lastClusterLength = result.Clusters[result.ClustersLength - 1].ToString().Length;
            result.Size -= ClusterSize - lastClusterLength;

            return result;
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
