using System;

namespace Type.BigInteger
{
    public class BigInteger : IComparable<BigInteger>
    {
        #region Constants

        private const int ClusterCapacity = 18;
        private const long MaxClusterValue = (long)1E18;

        #endregion


        #region Properties

        private bool IsNegative { get; set; }
        private bool IsZero { get; }
        private bool IsOne => ClustersLength == 1 && Clusters[Head] == 1;
        private int Head { get; set; }
        private int Tail { get; set; }
        public int ClustersLength => Tail - Head + 1;
        public int Length { get; private set; }
        private long[] Clusters { get; }

        #endregion


        #region Constructors

        public BigInteger(string input)
        {
            IsZero = input == "0";
            IsNegative = input[0] == '-';
            var offset = IsNegative ? 1 : 0;

            Length = input.Length - offset;
            Tail = (int)Math.Ceiling((double)Length / ClusterCapacity) - 1;
            Clusters = new long[ClustersLength];

            var clusterIndex = 0;
            for (var i = input.Length - ClusterCapacity - offset; i >= 0; i -= ClusterCapacity)
            {
                Clusters[clusterIndex] = Convert.ToInt64(input.Substring(i + offset, ClusterCapacity));
                clusterIndex++;
            }

            var remainder = Length % ClusterCapacity;
            if (remainder != 0) Clusters[clusterIndex] = Convert.ToInt64(input.Substring(offset, remainder));
        }

        private BigInteger(int clustersLength)
        {
            Length = clustersLength * ClusterCapacity;
            Tail = clustersLength - 1;
            Clusters = new long[ClustersLength];
        }

        private BigInteger(BigInteger bigInteger)
        {
            IsNegative = bigInteger.IsNegative;
            IsZero = bigInteger.IsZero;
            Length = bigInteger.Length;
            Head = bigInteger.Head;
            Tail = bigInteger.Tail;
            Clusters = (long[])bigInteger.Clusters.Clone();
        }

        #endregion


        #region Helper Methods

        private void RemoveLastCluster()
        {
            Tail--;
            Length -= ClusterCapacity;
        }

        private void RecomputeLength()
        {
            var lastClusterLength = Clusters[Tail].ToString().Length;
            Length = (ClustersLength - 1) * ClusterCapacity + lastClusterLength;
        }

        private void SplitClusters(int index, out BigInteger upper, out BigInteger lower)
        {
            index += Head;

            if (index >= Tail)
            {
                upper = new BigInteger("0");
                lower = this;
                return;
            }

            upper = new BigInteger(this) { Head = index + 1 };
            upper.RecomputeLength();

            lower = new BigInteger(this) { Tail = index };
            lower.RecomputeLength();
        }

        #endregion


        #region Arithmetic Methods

        public BigInteger Add(BigInteger other)
        {
            if (IsZero) return new BigInteger(other);
            if (other.IsZero) return new BigInteger(this);

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
                result = Subtract(other);
                other.IsNegative = true;
                return result;
            }

            var length = Math.Max(ClustersLength, other.ClustersLength);
            result = new BigInteger(length + 1);
            if (IsNegative && other.IsNegative) result.IsNegative = true;
            var carry = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i < ClustersLength ? Clusters[i + Head] : 0;
                var operand2 = i < other.ClustersLength ? other.Clusters[i + other.Head] : 0;
                var sum = operand1 + operand2 + carry;
                carry = sum >= MaxClusterValue ? 1 : 0;
                result.Clusters[i] = sum % MaxClusterValue;
            }

            result.Clusters[result.Tail] = carry;
            if (carry == 0) result.RemoveLastCluster(); // Remove Empty Cluster
            result.RecomputeLength();

            return result;
        }

        public BigInteger Subtract(BigInteger other)
        {
            if (IsZero) return new BigInteger(other) { IsNegative = false };
            if (other.IsZero) return new BigInteger(this);

            BigInteger result;

            if (IsNegative && !other.IsNegative)
            {
                other.IsNegative = true;
                result = Add(other);
                other.IsNegative = false;
                return result;
            }

            if (!IsNegative && other.IsNegative)
            {
                other.IsNegative = false;
                result = Add(other);
                other.IsNegative = true;
                return result;
            }

            var greater = this;
            var smaller = other;

            var length = Math.Max(ClustersLength, other.ClustersLength);
            result = new BigInteger(length);

            var comparison = CompareTo(other);
            if (comparison == 0) return new BigInteger("0");

            var isSmaller = comparison < 0;
            if (isSmaller)
            {
                result.IsNegative = true;
                if (!IsNegative)
                {
                    greater = other;
                    smaller = this;
                }
            }
            else if (IsNegative)
            {
                greater = other;
                smaller = this;
            }

            var borrow = 0;

            for (var i = greater.Head; i < length; i++)
            {
                var operand1 = i < greater.ClustersLength ? greater.Clusters[i + greater.Head] : 0;
                var operand2 = i < smaller.ClustersLength ? smaller.Clusters[i + smaller.Head] : 0;
                var diff = operand1 - operand2 - borrow;
                borrow = diff < 0 ? 1 : 0;
                result.Clusters[i] = diff + borrow * MaxClusterValue;
            }

            while (result.ClustersLength > 1 && result.Clusters[result.Tail] == 0)
            {
                result.RemoveLastCluster(); // Remove Empty Clusters
            }

            result.RecomputeLength();

            return result;
        }

        public BigInteger Multiply(BigInteger other)
        {
            if (IsZero || other.IsZero) return new BigInteger("0");
            if (IsOne) return new BigInteger(other);
            if (other.IsOne) return new BigInteger(this);
            if (ClustersLength == 1 && other.ClustersLength == 1)
            {
                var n = Math.Min(Length, other.Length);
                var m = (int)Math.Ceiling(n / 2.0);

                // Splitting
                var pow = (long)Math.Pow(10, m);
                var a = Clusters[Head] / pow;
                var b = Clusters[Head] % pow;
                var c = other.Clusters[other.Head] / pow;
                var d = other.Clusters[other.Head] % pow;

                var ac = a * c;
                var bd = b * d;
                var abcd = (a + b) * (c + d);

                // Shifting
                var operand1 = new BigInteger(ac + new string('0', 2 * m));
                var operand2 = new BigInteger(abcd - bd - ac + new string('0', m));

                var result = operand1.Add(operand2).Add(new BigInteger(bd.ToString()));
                result.IsNegative = IsNegative && !other.IsNegative || !IsNegative && other.IsNegative;
                return result;
            }
            else
            {
                var n = Math.Min(ClustersLength, other.ClustersLength);
                var m = (int)Math.Ceiling(n / 2.0);

                // Splitting
                SplitClusters(m - 1, out var a, out var b);
                other.SplitClusters(m - 1, out var c, out var d);

                var ac = a.Multiply(c);
                var bd = b.Multiply(d);
                var abcd = a.Add(b).Multiply(c.Add(d));

                // Shifting
                var operand1 = ac.ShiftLeft(2 * m * ClusterCapacity);
                var operand2 = abcd.Subtract(bd).Subtract(ac).ShiftLeft(m * ClusterCapacity);

                var result = operand1.Add(operand2).Add(bd);
                result.IsNegative = IsNegative && !other.IsNegative || !IsNegative && other.IsNegative;
                return result;
            }
        }

        public void Divide(BigInteger other, out BigInteger quotient, out BigInteger remainder)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Shifting Methods

        public BigInteger ShiftLeft(int count)
        {
            if (IsZero || count == 0) return new BigInteger(this);

            var clustersShifting = count / ClusterCapacity;
            var internalShifting = count % ClusterCapacity;
            var shiftBase = (long)Math.Pow(10, internalShifting);
            var split = MaxClusterValue / shiftBase;

            var result = new BigInteger(ClustersLength + clustersShifting + 1);
            long carry = 0;

            for (var index = 0; index < ClustersLength; index++)
            {
                var upper = Clusters[index + Head] / split;
                var lower = Clusters[index + Head] % split;
                var value = lower * shiftBase + carry;
                result.Clusters[index + clustersShifting] = value;
                carry = upper;
            }

            result.Clusters[result.Tail] = carry;
            if (carry == 0) result.RemoveLastCluster();
            result.RecomputeLength();
            result.IsNegative = IsNegative;

            return result;
        }

        public BigInteger ShiftRight(int count)
        {
            if (IsZero || count == 0 || Length <= ClusterCapacity) return new BigInteger(this);
            if (count >= Length) return new BigInteger("0");

            var clustersShifting = count / ClusterCapacity;
            var internalShifting = count % ClusterCapacity;
            var split = (long)Math.Pow(10, internalShifting);
            var shiftBase = MaxClusterValue / split;

            var result = new BigInteger(ClustersLength - clustersShifting);
            long carry = 0;

            for (var index = Tail; index >= clustersShifting && index < Tail; index--)
            {
                var upper = Clusters[index - Head] / split;
                var lower = Clusters[index - Head] % split;
                var value = upper + shiftBase * carry;
                result.Clusters[index - Head - clustersShifting] = value;
                carry = lower;
                if (value == 0) result.RemoveLastCluster();  // Remove Empty Cluster
            }

            result.RecomputeLength();
            result.IsNegative = IsNegative;

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
            if (IsZero && other.IsZero) return 0;
            if (IsNegative && !other.IsNegative) return -1;
            if (!IsNegative && other.IsNegative) return 1;
            if (ClustersLength > other.ClustersLength) return IsNegative ? -1 : 1;
            if (ClustersLength < other.ClustersLength) return IsNegative ? 1 : -1;
            for (var i = ClustersLength - 1; i >= 0; i--)
            {
                if (Clusters[i + Head] > other.Clusters[i + other.Head]) return IsNegative ? -1 : 1;
                if (Clusters[i + Head] < other.Clusters[i + other.Head]) return IsNegative ? 1 : -1;
            }
            return 0;
        }

        #endregion


        #region Equality Methods

        private bool Equals(BigInteger other)
        {
            if (IsZero && other.IsZero) return true;
            if (ClustersLength != other.ClustersLength) return false;
            for (var i = 0; i < ClustersLength; i++)
            {
                if (Clusters[i + Head] != other.Clusters[i + other.Head]) return false;
            }
            return IsNegative == other.IsNegative && Length == other.Length;
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
                str = Clusters[i + Head].ToString().PadLeft(ClusterCapacity, '0') + str;
            }

            return (IsNegative ? "-" : "") + Clusters[Tail] + str;
        }

        #endregion
    }
}
