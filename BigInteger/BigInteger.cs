using System;

namespace Type.BigInteger
{
    public class BigInteger : IComparable<BigInteger>
    {
        #region Constants

        private const int ClusterSize = 18;
        private const long MaxClusterValue = (long)1E18;

        #endregion


        #region Properties

        private bool IsNegative { get; set; }
        public int Size { get; private set; }
        public int ClustersLength { get; private set; }
        private long[] Clusters { get; }

        #endregion


        #region Constructors

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

        #endregion


        #region Helper Methods

        private void RemoveLastCluster()
        {
            ClustersLength--;
            Size -= ClusterSize;
        }

        private void RecomputeSize()
        {
            var lastClusterLength = Clusters[ClustersLength - 1].ToString().Length;
            Size = (ClustersLength - 1) * ClusterSize + lastClusterLength;
        }

        #endregion


        #region Arithmetic Methods

        public BigInteger Add(BigInteger other)
        {
            BigInteger result;

            if (IsNegative && !other.IsNegative)
            {
                IsNegative = false;
                result = other.Subtract(this);
                IsNegative = true;
                return result;
            }

            if (!IsNegative && other.IsNegative)
            {
                other.IsNegative = false;
                return Subtract(other);
            }

            var length = Math.Max(ClustersLength, other.ClustersLength);
            result = new BigInteger(length + 1);
            if (IsNegative && other.IsNegative) result.IsNegative = true;
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
            if (carry == 0) result.RemoveLastCluster(); // Remove Empty Cluster
            result.RecomputeSize();

            return result;
        }

        public BigInteger Subtract(BigInteger other)
        {
            if (IsNegative && !other.IsNegative)
            {
                other.IsNegative = true;
                return Add(other);
            }

            if (!IsNegative && other.IsNegative)
            {
                other.IsNegative = false;
                return Add(other);
            }

            var upper = this;
            var lower = other;

            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length);

            var comparison = CompareTo(other);
            if (comparison == 0) return new BigInteger("0");

            var isSmaller = comparison < 0;
            if (isSmaller)
            {
                result.IsNegative = true;
                if (!IsNegative)
                {
                    upper = other;
                    lower = this;
                }
            }
            else if (IsNegative)
            {
                upper = other;
                lower = this;
            }

            var borrow = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i < upper.ClustersLength ? upper.Clusters[i] : 0;
                var operand2 = i < lower.ClustersLength ? lower.Clusters[i] : 0;
                var diff = operand1 - operand2 - borrow;
                borrow = diff < 0 ? 1 : 0;
                result.Clusters[i] = diff + borrow * MaxClusterValue;
            }

            while (result.ClustersLength > 1 && result.Clusters[result.ClustersLength - 1] == 0)
            {
                result.RemoveLastCluster(); // Remove Empty Clusters
            }

            result.RecomputeSize();

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

        #endregion


        #region Shifting Methods

        public BigInteger ShiftLeft(int count)
        {
            var clustersShifting = count / ClusterSize;
            var internalShifting = count % ClusterSize;
            var shiftBase = (long)Math.Pow(10, internalShifting);
            var split = MaxClusterValue / shiftBase;

            var result = new BigInteger(ClustersLength + clustersShifting + 1);
            long carry = 0;

            for (var index = 0; index < ClustersLength; index++)
            {
                var upper = Clusters[index] / split;
                var lower = Clusters[index] % split;
                var value = lower * shiftBase + carry;
                result.Clusters[index + clustersShifting] = value;
                carry = upper;
            }

            result.Clusters[result.ClustersLength - 1] = carry;
            if (carry == 0) result.RemoveLastCluster();
            result.RecomputeSize();

            return result;
        }

        public BigInteger ShiftRight(int count)
        {
            if (count >= Size) return new BigInteger("0");

            var clustersShifting = count / ClusterSize;
            var internalShifting = count % ClusterSize;
            var split = (long)Math.Pow(10, internalShifting);
            var shiftBase = MaxClusterValue / split;

            var result = new BigInteger(ClustersLength - clustersShifting);
            long carry = 0;

            for (var index = ClustersLength - 1; index >= clustersShifting; index--)
            {
                var upper = Clusters[index] / split;
                var lower = Clusters[index] % split;
                var value = upper + shiftBase * carry;
                result.Clusters[index - clustersShifting] = value;
                carry = lower;
                if (value == 0) result.RemoveLastCluster();  // Remove Empty Cluster
            }

            result.RecomputeSize();
            return result;
        }

        #endregion


        #region Comparsion Methods

        /// <summary>
        /// Compares two objects
        /// </summary>
        /// <param name="other">Object to compare with</param>
        /// <returns>
        ///  0 if both are equal
        ///  1 if this is bigger than other
        /// -1 if other is bigger than this
        /// </returns>
        public int CompareTo(BigInteger other)
        {
            if (IsNegative && !other.IsNegative) return -1;
            if (!IsNegative && other.IsNegative) return 1;
            if (ClustersLength > other.ClustersLength) return IsNegative ? -1 : 1;
            if (ClustersLength < other.ClustersLength) return IsNegative ? 1 : -1;
            for (var i = 0; i < ClustersLength; i++)
            {
                if (Clusters[i] > other.Clusters[i]) return IsNegative ? -1 : 1;
                if (Clusters[i] < other.Clusters[i]) return IsNegative ? 1 : -1;
            }
            return 0;
        }

        #endregion


        #region Equality Methods

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
            var hashCode = Clusters != null ? Clusters.GetHashCode() : 0;
            return hashCode;
        }

        #endregion


        #region Formatting Methods

        public override string ToString()
        {
            var str = "";
            for (var i = 0; i < ClustersLength - 1; i++)
            {
                str = Clusters[i].ToString().PadLeft(ClusterSize, '0') + str;
            }

            return (IsNegative ? "-" : "") + Clusters[ClustersLength - 1] + str;
        }

        #endregion
    }
}
