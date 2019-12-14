using System;

namespace Type.BigInteger
{
    public class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>
    {
        #region Constants

        private const int ClusterCapacity = 18;
        private const long MaxClusterValue = (long)1E18;

        #endregion


        #region Static Members

        private static readonly BigInteger BigZero = new BigInteger("0");
        private static readonly BigInteger BigOne = new BigInteger("1");

        #endregion


        #region Properties

        private bool IsNegative { get; set; }
        private bool IsZero => ClustersLength == 1 && Clusters[0] == 0;
        private bool IsOne => !IsNegative && ClustersLength == 1 && Clusters[0] == 1;
        public int ClustersLength { get; private set; }
        private int Tail => ClustersLength - 1;
        public int Length { get; private set; }
        private long[] Clusters { get; set; }

        #endregion


        #region Constructors

        private BigInteger() { }

        public BigInteger(string input)
        {
            IsNegative = input[0] == '-';
            var offset = IsNegative ? 1 : 0;

            Length = input.Length - offset;
            ClustersLength = (int)Math.Ceiling((double)Length / ClusterCapacity);
            Clusters = new long[ClustersLength];

            var clusterIndex = 0;
            for (var i = Length - ClusterCapacity; i >= 0; i -= ClusterCapacity)
            {
                Clusters[clusterIndex] = Convert.ToInt64(input.Substring(i + offset, ClusterCapacity));
                clusterIndex++;
            }

            var remainder = Length % ClusterCapacity;
            if (remainder != 0) Clusters[clusterIndex] = Convert.ToInt64(input.Substring(offset, remainder));
            RemoveEmptyClusters();
            RecomputeLength();
        }

        private BigInteger(int clustersLength, bool isNegative = false)
        {
            Length = clustersLength * ClusterCapacity;
            ClustersLength = clustersLength;
            Clusters = new long[ClustersLength];
            IsNegative = isNegative;
        }

        #endregion


        #region Helper Methods

        private void RemoveLastCluster()
        {
            ClustersLength--;
            Length -= ClusterCapacity;
        }

        private void RemoveEmptyClusters()
        {
            while (ClustersLength > 1 && Clusters[Tail] == 0)
            {
                RemoveLastCluster();
            }
        }

        private void RecomputeLength()
        {
            var lastClusterLength = Clusters[Tail].ToString().Length;
            Length = (Tail) * ClusterCapacity + lastClusterLength;
        }

        private (BigInteger, BigInteger) SplitClusters(int index)
        {
            if (index >= ClustersLength || ClustersLength == 1) return (BigZero.Clone(), Clone());

            var (upper, lower) = (Clone(start: index + 1), Clone(end: index));
            upper.RecomputeLength();
            lower.RecomputeLength();

            return (upper, lower);
        }

        private BigInteger Clone(bool? isNegative = null, int? start = null, int? end = null)
        {
            var newStart = start ?? 0;
            var newEnd = end ?? Tail;
            var clustersLength = newEnd - newStart + 1;
            var bigInteger = new BigInteger
            {
                IsNegative = isNegative ?? IsNegative,
                ClustersLength = clustersLength,
                Clusters = new long[clustersLength],
                Length = Length
            };
            for (var i = 0; i < clustersLength; i++)
            {
                bigInteger.Clusters[i] = Clusters[i + newStart];
            }

            return bigInteger;
        }

        #endregion


        #region Arithmetic Methods

        private BigInteger Add(BigInteger other)
        {
            if (IsZero) return other.Clone();
            if (other.IsZero) return Clone();

            if (IsNegative && !other.IsNegative) return other.Subtract(Clone(!IsNegative));
            if (!IsNegative && other.IsNegative) return Subtract(other.Clone(!other.IsNegative));

            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length + 1, IsNegative && other.IsNegative);
            var carry = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i <= Tail ? Clusters[i] : 0;
                var operand2 = i <= other.Tail ? other.Clusters[i] : 0;
                var sum = operand1 + operand2 + carry;
                carry = sum >= MaxClusterValue ? 1 : 0;
                result.Clusters[i] = sum % MaxClusterValue;
            }

            result.Clusters[result.Tail] = carry;
            if (carry == 0) result.RemoveLastCluster();
            result.RecomputeLength();

            return result;
        }

        private BigInteger Subtract(BigInteger other)
        {
            if (IsZero) return other.Clone(false);
            if (other.IsZero) return Clone();

            if (IsNegative ^ other.IsNegative) return Add(other.Clone(!other.IsNegative));

            var comparison = CompareTo(other);
            if (comparison == 0) return BigZero.Clone();
            var isSmaller = comparison < 0;

            var greater = this;
            var smaller = other;

            if (isSmaller ^ IsNegative)
            {
                greater = other;
                smaller = this;
            }

            var length = Math.Max(ClustersLength, other.ClustersLength);
            var result = new BigInteger(length, isSmaller);
            var borrow = 0;

            for (var i = 0; i < length; i++)
            {
                var operand1 = i <= greater.Tail ? greater.Clusters[i] : 0;
                var operand2 = i <= smaller.Tail ? smaller.Clusters[i] : 0;
                var diff = operand1 - operand2 - borrow;
                borrow = diff < 0 ? 1 : 0;
                result.Clusters[i] = diff + borrow * MaxClusterValue;
            }

            result.RemoveEmptyClusters();
            result.RecomputeLength();

            return result;
        }

        private BigInteger Multiply(BigInteger other)
        {
            if (IsZero || other.IsZero) return BigZero.Clone();
            if (IsOne) return other.Clone();
            if (other.IsOne) return Clone();
            if (ClustersLength == 1 && other.ClustersLength == 1)
            {
                var minLength = Math.Min(Length, other.Length);
                var splitIndex = (int)Math.Ceiling(minLength / 2.0);
                var splitBase = (long)Math.Pow(10, splitIndex);

                var upper1 = Clusters[0] / splitBase;
                var lower1 = Clusters[0] % splitBase;
                var upper2 = other.Clusters[0] / splitBase;
                var lower2 = other.Clusters[0] % splitBase;

                var uppers = upper1 * upper2;
                var lowers = lower1 * lower2;
                var middle = (upper1 + lower1) * (upper2 + lower2) - uppers - lowers;

                var shiftedUppers = uppers + new string('0', splitIndex * 2);
                var shiftedMiddle = middle + new string('0', splitIndex);

                var result = new BigInteger(shiftedUppers) + new BigInteger(shiftedMiddle) + new BigInteger($"{lowers}");
                result.IsNegative = IsNegative ^ other.IsNegative;
                return result;
            }
            else
            {
                var minLength = Math.Min(ClustersLength, other.ClustersLength);
                var splitIndex = (int)Math.Ceiling(minLength / 2.0);

                var (upper1, lower1) = SplitClusters(splitIndex - 1);
                var (upper2, lower2) = other.SplitClusters(splitIndex - 1);

                var uppers = upper1 * upper2;
                var lowers = lower1 * lower2;
                var middle = (upper1 + lower1) * (upper2 + lower2) - uppers - lowers;

                var shiftedUppers = uppers << (splitIndex * ClusterCapacity * 2);
                var shiftedMiddle = middle << (splitIndex * ClusterCapacity);

                var result = shiftedUppers + shiftedMiddle + lowers;
                result.IsNegative = IsNegative ^ other.IsNegative;
                return result;
            }
        }

        private (BigInteger, BigInteger) Divide(BigInteger other)
        {
            if (other.IsZero) throw new DivideByZeroException("You Can't Divide By Zero Einstein");

            var signs = (IsNegative, other.IsNegative);     // Save Signs
            IsNegative = other.IsNegative = false;          // Clear Signs

            var (quotient, remainder) = DividePositive(other);
            quotient.IsNegative = signs.Item1 ^ signs.Item2;

            (IsNegative, other.IsNegative) = signs;         // Restore Signs
            return (quotient, remainder);
        }

        private (BigInteger, BigInteger) DividePositive(BigInteger other)
        {
            if (other.IsOne) return (Clone(), BigZero.Clone());
            if (this < other) return (BigZero.Clone(), Clone());
            var (quotient, remainder) = DividePositive(other + other);
            quotient += quotient;
            return remainder < other ? (quotient, remainder) : (++quotient, remainder - other);
        }

        #endregion


        #region Shifting Methods

        public BigInteger ShiftLeft(int count)
        {
            if (IsZero || count == 0) return Clone();

            var clustersShifting = count / ClusterCapacity;
            var internalShifting = count % ClusterCapacity;
            var shiftBase = (long)Math.Pow(10, internalShifting);
            var split = MaxClusterValue / shiftBase;

            var result = new BigInteger(ClustersLength + clustersShifting + 1, IsNegative);
            long carry = 0;

            for (var i = 0; i < ClustersLength; i++)
            {
                var upper = Clusters[i] / split;
                var lower = Clusters[i] % split;
                var value = lower * shiftBase + carry;
                result.Clusters[i + clustersShifting] = value;
                carry = upper;
            }

            result.Clusters[result.Tail] = carry;
            if (carry == 0) result.RemoveLastCluster();
            result.RecomputeLength();

            return result;
        }

        public BigInteger ShiftRight(int count)
        {
            if (IsZero || count == 0 || Length <= ClusterCapacity) return Clone();
            if (count >= Length) return BigZero.Clone();

            var clustersShifting = count / ClusterCapacity;
            var internalShifting = count % ClusterCapacity;
            var split = (long)Math.Pow(10, internalShifting);
            var shiftBase = MaxClusterValue / split;

            var result = new BigInteger(ClustersLength - clustersShifting, IsNegative);
            long carry = 0;

            for (var i = Tail; i >= clustersShifting; i--)
            {
                var upper = Clusters[i] / split;
                var lower = Clusters[i] % split;
                var value = upper + shiftBase * carry;
                result.Clusters[i - clustersShifting] = value;
                carry = lower;
            }

            result.RemoveEmptyClusters();
            result.RecomputeLength();

            return result;
        }

        #endregion


        #region Operators


        #region Arithmetic

        public static BigInteger operator ++(BigInteger bigInteger) => bigInteger.Add(BigOne);

        public static BigInteger operator --(BigInteger bigInteger) => bigInteger.Subtract(BigOne);


        public static BigInteger operator +(BigInteger left, BigInteger right) => left.Add(right);

        public static BigInteger operator +(BigInteger left, int right) => left.Add(new BigInteger($"{right}"));


        public static BigInteger operator -(BigInteger left, BigInteger right) => left.Subtract(right);

        public static BigInteger operator -(BigInteger left, int right) => left.Subtract(new BigInteger($"{right}"));


        public static BigInteger operator *(BigInteger left, BigInteger right) => left.Multiply(right);

        public static BigInteger operator *(BigInteger left, int right) => left.Multiply(new BigInteger($"{right}"));


        public static BigInteger operator /(BigInteger left, BigInteger right) => left.Divide(right).Item1;

        public static BigInteger operator /(BigInteger left, int right) => left.Divide(new BigInteger($"{right}")).Item1;

        public static BigInteger operator %(BigInteger left, BigInteger right) => left.Divide(right).Item2;

        public static BigInteger operator %(BigInteger left, int right) => left.Divide(new BigInteger($"{right}")).Item2;

        #endregion


        #region Bitwise

        public static BigInteger operator <<(BigInteger left, int right) => left.ShiftLeft(right);

        public static BigInteger operator >>(BigInteger left, int right) => left.ShiftRight(right);

        #endregion


        #region Comparison

        public static bool operator ==(BigInteger left, BigInteger right) => left.Equals(right);

        public static bool operator !=(BigInteger left, BigInteger right) => !left.Equals(right);


        public static bool operator <(BigInteger left, BigInteger right) => left.CompareTo(right) < 0;

        public static bool operator >(BigInteger left, BigInteger right) => left.CompareTo(right) > 0;

        public static bool operator <=(BigInteger left, BigInteger right) => left.CompareTo(right) <= 0;

        public static bool operator >=(BigInteger left, BigInteger right) => left.CompareTo(right) >= 0;

        #endregion


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
            for (var i = Tail; i >= 0; i--)
            {
                if (Clusters[i] > other.Clusters[i]) return IsNegative ? -1 : 1;
                if (Clusters[i] < other.Clusters[i]) return IsNegative ? 1 : -1;
            }
            return 0;
        }

        #endregion


        #region Equality Methods

        public bool Equals(BigInteger other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ClustersLength != other.ClustersLength) return false;
            if (IsNegative != other.IsNegative) return false;
            if (Length != other.Length) return false;
            for (var i = 0; i < ClustersLength; i++)
            {
                if (Clusters[i] != other.Clusters[i]) return false;
            }

            return true;
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
                hashCode = (hashCode * 397) ^ ClustersLength;
                hashCode = (hashCode * 397) ^ Length;
                hashCode = (hashCode * 397) ^ (Clusters != null ? Clusters.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion


        #region Formatting Methods

        public override string ToString()
        {
            var str = "";
            for (var i = 0; i < Tail; i++)
            {
                str = Clusters[i].ToString().PadLeft(ClusterCapacity, '0') + str;
            }

            return (IsNegative && !IsZero ? "-" : "") + Clusters[Tail] + str;
        }

        #endregion
    }
}