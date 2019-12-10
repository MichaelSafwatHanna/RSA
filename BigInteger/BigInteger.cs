using System;

namespace Type.BigInteger
{
    public class BigInteger : IComparable<BigInteger>
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
        private bool IsZero => ClustersLength == 1 && Clusters[Head] == 0;
        private bool IsOne => !IsNegative && ClustersLength == 1 && Clusters[Head] == 1;
        private int Head { get; set; }
        private int Tail { get; set; }
        public int ClustersLength => Tail - Head + 1;
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
            RemoveEmptyClusters();
            RecomputeLength();
        }

        private BigInteger(int clustersLength)
        {
            Length = clustersLength * ClusterCapacity;
            Tail = clustersLength - 1;
            Clusters = new long[ClustersLength];
        }

        #endregion


        #region Helper Methods

        private void RemoveLastCluster()
        {
            Tail--;
            Length -= ClusterCapacity;
        }

        private void RemoveEmptyClusters()
        {
            while (Tail > Head && Clusters[Tail] == 0)
            {
                RemoveLastCluster();
            }
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
                upper = BigZero.Clone();
                lower = Clone();
                return;
            }

            upper = Clone(head: index + 1);
            upper.RecomputeLength();

            lower = Clone(tail: index);
            lower.RecomputeLength();
        }

        private BigInteger Clone(long[] clusters = null, bool? isNegative = null,
                                 int? length = null, int? head = null, int? tail = null)
        {
            return new BigInteger
            {
                Clusters = clusters ?? (long[])Clusters.Clone(),
                IsNegative = isNegative ?? IsNegative,
                Length = length ?? Length,
                Head = head ?? Head,
                Tail = tail ?? Tail
            };
        }

        #endregion


        #region Arithmetic Methods

        private BigInteger Add(BigInteger other)
        {
            if (IsZero) return other.Clone();
            if (other.IsZero) return Clone();

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
            result = new BigInteger(length + 1) { IsNegative = IsNegative && other.IsNegative };
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
            if (carry == 0) result.RemoveLastCluster();
            result.RecomputeLength();

            return result;
        }

        private BigInteger Subtract(BigInteger other)
        {
            if (IsZero) return other.Clone(isNegative: false);
            if (other.IsZero) return Clone();

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
            if (comparison == 0) return BigZero.Clone();

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

                var upper1 = Clusters[Head] / splitBase;
                var lower1 = Clusters[Head] % splitBase;
                var upper2 = other.Clusters[other.Head] / splitBase;
                var lower2 = other.Clusters[other.Head] % splitBase;

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

                SplitClusters(splitIndex - 1, out var upper1, out var lower1);
                other.SplitClusters(splitIndex - 1, out var upper2, out var lower2);

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

        private void Divide(BigInteger other, out BigInteger quotient, out BigInteger remainder)
        {
            if (other.IsZero) throw new DivideByZeroException("You Can't Divide By Zero Einstein");

            // Save Signs
            var memorizedSign1 = IsNegative;
            var memorizedSign2 = other.IsNegative;
            IsNegative = other.IsNegative = false;

            DividePositive(other, out quotient, out remainder);

            // Restore Signs
            IsNegative = memorizedSign1;
            other.IsNegative = memorizedSign2;
            quotient.IsNegative = IsNegative ^ other.IsNegative;
        }

        private void DividePositive(BigInteger other, out BigInteger quotient, out BigInteger remainder)
        {
            if (other.IsOne)
            {
                quotient = Clone();
                remainder = BigZero.Clone();
                return;
            }

            if (this < other)
            {
                quotient = BigZero.Clone();
                remainder = Clone();
                return;
            }

            DividePositive(other * 2, out quotient, out remainder);
            quotient *= 2;
            if (remainder < other) return;
            quotient++;
            remainder -= other;
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
            if (IsZero || count == 0 || Length <= ClusterCapacity) return Clone();
            if (count >= Length) return BigZero.Clone();

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


        #region Operators

        #region Arithmetic

        public static BigInteger operator +(BigInteger left, BigInteger right) => left.Add(right);

        public static BigInteger operator +(BigInteger left, int right) => left.Add(new BigInteger($"{right}"));


        public static BigInteger operator -(BigInteger left, BigInteger right) => left.Subtract(right);

        public static BigInteger operator -(BigInteger left, int right) => left.Subtract(new BigInteger($"{right}"));


        public static BigInteger operator *(BigInteger left, BigInteger right) => left.Multiply(right);

        public static BigInteger operator *(BigInteger left, int right) => left.Multiply(new BigInteger($"{right}"));


        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            left.Divide(right, out var quotient, out _);
            return quotient;
        }

        public static BigInteger operator /(BigInteger left, int right)
        {
            left.Divide(new BigInteger($"{right}"), out var quotient, out _);
            return quotient;
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            left.Divide(right, out _, out var remainder);
            return remainder;
        }
        public static BigInteger operator %(BigInteger left, int right)
        {
            left.Divide(new BigInteger($"{right}"), out _, out var remainder);
            return remainder;
        }

        public static BigInteger operator ++(BigInteger bigInteger) => bigInteger.Add(BigOne);

        public static BigInteger operator --(BigInteger bigInteger) => bigInteger.Subtract(BigOne);


        #endregion


        #region Comparison

        public static BigInteger operator <<(BigInteger left, int right) => left.ShiftLeft(right);

        public static BigInteger operator >>(BigInteger left, int right) => left.ShiftRight(right);


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

            return (IsNegative && !IsZero ? "-" : "") + Clusters[Tail] + str;
        }

        #endregion
    }
}
