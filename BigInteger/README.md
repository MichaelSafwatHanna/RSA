# BigInteger

<!-- TOC depthFrom:2 depthTo:4 -->

- [1. Data Structure](#1-data-structure)
    - [1.1. Clusters](#11-clusters)
    - [1.2. Flags](#12-flags)
    - [1.3. Example](#13-example)
- [2. Code Analysis](#2-code-analysis)
    - [2.1. Helper Methods](#21-helper-methods)
        - [2.1.1. RecomputeLength](#211-recomputelength)
        - [2.1.2. RemoveLastCluster](#212-removelastcluster)
        - [2.1.3. RemoveEmptyClusters](#213-removeemptyclusters)
        - [2.1.4. SplitClusters](#214-splitclusters)
        - [2.1.5. Clone](#215-clone)
        - [2.1.6. CompareTo](#216-compareto)
    - [2.2. Primary Constructor](#22-primary-constructor)
    - [2.3. Secondary Constructor](#23-secondary-constructor)
    - [2.4. ShiftLeft](#24-shiftleft)
    - [2.5. Add](#25-add)
    - [2.6. Subtract](#26-subtract)
    - [2.7. Multiply](#27-multiply)
    - [2.8. Divide](#28-divide)
    - [2.9. ModOfPower](#29-modofpower)

<!-- /TOC -->

## 1. Data Structure

### 1.1. Clusters

The input's length is saved in `Length` then it is divided into `long` array (`Clusters`) in **reverse order** where maximum cluster value < **1E18** _(Maximum length of `long` - 1 to avoid overflowing)_.

### 1.2. Flags

| Flag     | Description                              | Purpose        |
| -------- | ---------------------------------------- | -------------- |
| Negative | Indicates negativity of the BigInteger   | General        |
| Zero     | Indicates whether the BigInteger is zero | General        |
| One      | Indicates whether the BigInteger is one  | Multiplication |

### 1.3. Example

**Input:** "111111111 222222222 333333333 444444444 555555555 666666666 777777777 888888888 000000000"

- `ClusterCapacity` = 18
- `Length` = 81
- `ClustersLength` = ⌈`Length` / `ClusterCapacity`⌉
- `TAIL` = `ClustersLength` - 1
- `isNegative` = false
- `isZero` = false
- `isOne` = false

|        [0]         |        [1]         |        [2]         |        [3]         | [4] `TAIL` |
| :----------------: | :----------------: | :----------------: | :----------------: | :--------: |
| 888888888000000000 | 666666666777777777 | 444444444555555555 | 222222222333333333 | 111111111  |

## 2. Code Analysis

### 2.1. Helper Methods

#### 2.1.1. RecomputeLength

```c#
// Total Complexity: O(1)
BigInteger.RecomputeLength()
{
    lastClusterLength = Clusters[Tail].ToString().Length
    Length = Tail * ClusterCapacity + lastClusterLength
}
```

#### 2.1.2. RemoveLastCluster

```c#
// Total Complexity: O(1)
BigInteger.RemoveLastCluster()
{
    ClustersLength--
    Length -= ClusterCapacity
}
```

#### 2.1.3. RemoveEmptyClusters

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
BigInteger.RemoveEmptyClusters()
{
    while (ClustersLength > 1 && Clusters[Tail] == 0)                                    // O(N * Body) = O(N)
    {
        RemoveLastCluster()                                                              // O(1)
    }
}
```

#### 2.1.4. SplitClusters

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
BigInteger.SplitClusters(index)
{
    if (index >= ClustersLength || ClustersLength == 1) return (BigZero, Clone())        // O(N)
    (upper, lower) = (Clone(start: index + 1), Clone(end: index))                        // O(N/2) + O(N/2) = O(N)
    upper.RecomputeLength()                                                              // O(1)
    lower.RecomputeLength()                                                              // O(1)
    return (upper, lower)                                                                // O(1)
}
```

#### 2.1.5. Clone

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
BigInteger.Clone(isNegative, start, end)
{
    bigInteger = new BigInteger
    {
        IsNegative = isNegative,                                                         // O(1)
        ClustersLength = end - start + 1,                                                // O(1)
        Clusters = new long[ClustersLength],                                             // O(1)
        Length = Length                                                                  // O(1)
    }

    for (i = 0 to bigInteger.ClustersLength - 1)                                         // O(N * Body) = O(N)
    {
        bigInteger.Clusters[i] = Clusters[i + start]                                     // O(1)
    }

    return bigInteger                                                                    // O(1)
}
```

#### 2.1.6. CompareTo

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
BigInteger.CompareTo(y)
{
    if (IsZero && y.IsZero) return 0                                                     // O(1)
    if (IsNegative && !y.IsNegative) return -1                                           // O(1)
    if (!IsNegative && y.IsNegative) return 1                                            // O(1)
    if (ClustersLength > y.ClustersLength) return IsNegative ? -1 : 1                    // O(1)
    if (ClustersLength < y.ClustersLength) return IsNegative ? 1 : -1                    // O(1)
    for (i = Tail to 0)                                                                  // O(N * Body) = O(N)
    {
        if (Clusters[i] > y.Clusters[i]) return IsNegative ? -1 : 1                      // O(1)
        if (Clusters[i] < y.Clusters[i]) return IsNegative ? 1 : -1                      // O(1)
    }
    return 0                                                                             // O(1)
}
```

### 2.2. Primary Constructor

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
BigInteger(input)
{
    IsNegative = input[0] == '-'                                                         // O(1)
    offset = IsNegative ? 1 : 0                                                          // O(1)

    Length = input.Length - offset                                                       // O(1)
    ClustersLength = Ceiling(Length / ClusterCapacity)                                   // O(1)
    Clusters = new long[ClustersLength]                                                  // O(1)

    clusterIndex = 0;                                                                    // O(1)
    for (var i = Length - ClusterCapacity; i >= 0; i -= ClusterCapacity)                 // O(N * Body) = O(N)
    {
        Clusters[clusterIndex] = (long) input.Substring(i + offset, ClusterCapacity)     // O(1)
        clusterIndex++                                                                   // O(1)
    }

    remainder = Length % ClusterCapacity;                                                // O(1)
    if (remainder != 0) Clusters[Tail] = (long) input.Substring(offset, remainder)       // O(1)
    RemoveEmptyClusters()                                                                // O(N)
    RecomputeLength()                                                                    // O(1)
}
```

### 2.3. Secondary Constructor

```c#
// Total Complexity: O(1)
BigInteger(clustersLength, isNegative)
{
    Length = clustersLength * ClusterCapacity;
    ClustersLength = clustersLength;
    Clusters = new long[clustersLength];
    IsNegative = isNegative;
}
```

### 2.4. ShiftLeft

```c#
/*
 * n: Number of digits of input
 * N: Number of clusters of input (i.e. n / 18)
 * Total Complexity: O(N)
 */
ShiftLeft(x, count)
{
    if (x.IsZero || count == 0) return x;

    clustersShifting = count / ClusterCapacity                                           // O(1)
    internalShifting = count % ClusterCapacity                                           // O(1)
    shiftBase = Pow(10, internalShifting)                                                // O(1)
    split = MaxClusterValue / shiftBase                                                  // O(1)

    result = new BigInteger(x.ClustersLength + clustersShifting + 1, x.IsNegative)       // O(1)
    carry = 0                                                                            // O(1)

    for (i = 0 to x.ClustersLength - 1)                                                  // O(N * Body) = O(N)
    {
        upper = x.Clusters[i] / split                                                    // O(1)
        lower = x.Clusters[i] % split                                                    // O(1)
        value = lower * shiftBase + carry                                                // O(1)
        result.Clusters[i + clustersShifting] = value                                    // O(1)
        carry = upper                                                                    // O(1)
    }

    result.Clusters[result.Tail] = carry                                                 // O(1)
    if (carry == 0) result.RemoveLastCluster()                                           // O(1)
    result.RecomputeLength()                                                             // O(1)

    return result                                                                        // O(1)
}
```

### 2.5. Add

```c#
/*
 * n: Number of digits of larger input
 * N: Number of clusters of larger input (i.e. n / 18)
 * Total Complexity: O(N)
 */
Add(x, y)
{
    if (x.IsZero) return y                                                               // O(1)
    if (y.IsZero) return x                                                               // O(1)

    if (x.IsNegative and !y.IsNegative) return Subtract(y, x.Clone(false))               // O(N)
    if (!x.IsNegative and y.IsNegative) return Subtract(x, y.Clone(false))               // O(N)

    length = Max(x.ClustersLength, y.ClustersLength)                                     // O(1)
    result = new BigInteger(length + 1, x.IsNegative and y.IsNegative)                   // O(1)

    carry = 0                                                                            // O(1)
    for (i = 0 to length)                                                                // O(N * Body) = O(N)
    {
        operand1 = i <= x.Tail ? x.Clusters[i] : 0                                       // O(1)
        operand2 = i <= y.Tail ? y.Clusters[i] : 0                                       // O(1)
        sum = operand1 + operand2 + carry                                                // O(1)
        carry = sum >= MaxClusterValue ? 1 : 0                                           // O(1)
        result.Clusters[i] = sum % MaxClusterValue                                       // O(1)
    }

    result.Clusters[result.Tail] = carry                                                 // O(1)
    if (carry == 0) result.RemoveLastCluster()                                           // O(1)
    result.RecomputeLength()                                                             // O(1)
    return result                                                                        // O(1)
}
```

### 2.6. Subtract

```c#
/*
 * n: Number of digits of larger input
 * N: Number of clusters of larger input (i.e. n / 18)
 * Total Complexity: O(N)
 */
Subtract(x, y)
{
    if (x.IsZero) return  y.Clone(isNegative: false)                                     // O(1)
    if (y.IsZero) return x                                                               // O(1)
    if (x.IsNegative xor y.IsNegative) return Add(x, y.Clone(!isNegative))               // O(N)

    comparison = x.CompareTo(y)                                                          // O(N)
    if (comparison == 0) return BigZero                                                  // O(1)
    isSmaller = comparison < 0                                                           // O(1)

    if (isSmaller xor IsNegative) Swap(x, y)                                             // O(1)

    length = Max(x.ClustersLength, y.ClustersLength)                                     // O(1)
    result = new BigInteger(length, isSmaller)                                           // O(1)
    borrow = 0                                                                           // O(1)

    for (i = 0 to length - 1)                                                            // O(N * Body) = O(N)
    {
        operand1 = i <= x.Tail ? x.Clusters[i] : 0                                       // O(1)
        operand2 = i <= y.Tail ? y.Clusters[i] : 0                                       // O(1)
        diff = operand1 - operand2 - borrow                                              // O(1)
        borrow = diff < 0 ? 1 : 0                                                        // O(1)
        result.Clusters[i] = diff + borrow * MaxClusterValue                             // O(1)
    }

    result.RemoveEmptyClusters()                                                         // O(N)
    result.RecomputeLength()                                                             // O(1)

    return result                                                                        // O(1)
}
```

### 2.7. Multiply

```c#
/*
 * n: Number of digits of larger input
 * N: Number of clusters of larger input (i.e. n / 18)
 * Total Complexity: Θ(N¹·⁵⁸)
 */
Multiply(x, y)
{
    if (x.IsZero or y.IsZero) return BigZero                                             // O(1)
    if (x.IsOne) return y                                                                // O(1)
    if (y.IsOne) return x                                                                // O(1)

    if (x.ClustersLength == 1 and y.ClustersLength == 1)                                 // O(1)
    {
        minLength = Min(x.Length, y.Length)                                              // O(1)
        splitIndex = Ceiling(minLength / 2)                                              // O(1)
        splitBase = Pow(10, splitIndex)                                                  // O(1)

        upperX = x.Clusters[0] / splitBase                                               // O(1)
        lowerX = x.Clusters[0] % splitBase                                               // O(1)
        upperY = y.Clusters[0] / splitBase                                               // O(1)
        lowerY = y.Clusters[0] % splitBase                                               // O(1)

        uppers = upperX * upperY                                                         // O(1)
        lowers = lowerX * lowerY                                                         // O(1)
        middle = (upperX + lowerX) * (upperY + lowerY) - uppers - lowers                 // O(1)

        shiftedUppers = uppers + new string('0', splitIndex * 2)                         // O(1)
        shiftedMiddle = middle + new string('0', splitIndex)                             // O(1)

        bigInt1 = new BigInteger(shiftedUppers)                                          // O(1) : 2 Clusters
        bigInt2 = new BigInteger(shiftedMiddle)                                          // O(1) : 1 Cluster
        bigInt3 = new BigInteger(lowers.ToString())                                      // O(1) : 1 Cluster

        result = Add(bigInt1, bigInt2)                                                   // O(1) : 2 Clusters max.
        result = Add(result, bigInt3)                                                    // O(1) : 3 Clusters max.
        result.IsNegative = x.IsNegative xor y.IsNegative                                // O(1)
        return result                                                                    // O(1)
    }
    else
    {
        minLength = Min(x.ClustersLength, y.ClustersLength)                              // O(1)
        splitIndex = Ceiling(minLength / 2)                                              // O(1)

        (upperX, lowerX) = SplitClusters(x, splitIndex - 1)                              // O(N)
        (upperY, lowerY) = SplitClusters(y, splitIndex - 1)                              // O(N)

        add1 = Add(upperX, lowerX)                                                       // O(N)
        add2 = Add(upperY, lowerY)                                                       // O(N)
        uppers = Multiply(upperX, upperY)                                                // [★]
        lowers = Multiply(lowerX, lowerY)                                                // [★]
        middle = Multiply(add1, add2)                                                    // [★]
        middle = Subtract(middle, uppers)                                                // O(N)
        middle = Subtract(middle, lowers)                                                // O(N)

        shiftedUppers = ShiftLeft(uppers, splitIndex * ClusterCapacity * 2)              // O(N)
        shiftedMiddle = ShiftLeft(middle, splitIndex * ClusterCapacity)                  // O(N)

        result = Add(shiftedUppers, shiftedMiddle)                                       // O(N)
        result = Add(result, lowers)                                                     // O(N)
        result.IsNegative = x.IsNegative xor y.IsNegative                                // O(1)
        return result                                                                    // O(1)
    }
}
/* [★]:
 *      T(Base Case)          = O(1)
 *      T(Recursive Code)     = 3T(N/2)
 *      T(Non-Recursive Code) = 10N
 *      T(N) = T(Recursive Code) + T(Non-Recursive Code)
 *      T(N) = 3T(N/2) + 10N
 *
 *      Using Master Method:
 *
 *           a = 3,   b = 2,   f(N) = Θ(N)
 *           log₂(3) = 1.58
 *        ∴  N¹·⁵⁸ ⁻ ᵉ > f(N)         Case (1)
 *           where: 0 < e < 0.58
 *        ∴  T(N) = Θ(N¹·⁵⁸)
 */
```

### 2.8. Divide

```c#
/*
 * n: Number of digits of x
 * N: Number of clusters of x (i.e. n / 18)
 * m: Number of digits of y
 * M: Number of clusters of y (i.e. m / 18)
 * Total Complexity: O(N)
 */
Divide(x, y)
{
    if (y.IsZero) error("Divide by Zero")                                                // O(1)
    if (y.IsOne) return (x, BigZero)                                                     // O(1)
    if (x.CompareTo(y) < 0)                                                              // O(N)
        return (BigZero, x)                                                              // O(1)
    y = Add(y, y)                                                                        // O(N)
    (quotient, remainder) = Divide(y)                                                    // [★]
    quotient = Add(quotient, quotient)                                                   // O(N)
    if (remainder.CompareTo(y) < 0)                                                      // O(N)
        return (quotient, remainder)                                                     // O(1)
    return (Add(quotient, 1), Subtract(remainder, y))                                    // O(2N)
}
/* [★]:
 *      T(Recursive Code)     = T(2N)
 *      T(Non-Recursive Code) = 6N
 *      T(N) = T(Recursive Code) + T(Non-Recursive Code)
 *      T(N) = T(2N) + 6N
 *
 *      Using Iterative Method:
 *
 *          T(N)  = T(2N) +  6N      -> [1]
 *          T(2N) = T(4N) + 12N
 *          T(N)  = T(4N) + 18N      -> [2]
 *          T(4N) = T(8N) + 24N
 *          T(N)  = T(8N) + 42N      -> [3]
 *
 *       ∴  General Form  : T(N) = T(2ᵏ N) + 6 (2ᵏ-1) N         (where K is iteration number)
 *          and Base Case : T(M) = 1
 *
 *      By Substitution:
 *
 *          T(2ᵏ N) = T(M)
 *          2ᵏ N    = M
 *          2ᵏ      = M/N
 *          K       = log₂(M/N)
 *
 *       ∴  T(N)  = 1 + 6 (2⁽ˡᵒᵍ₂ᴹ⸍ᴺ⁾ - 1) N
 *          T(N)  = O(M-N)
 */
```

### 2.9. ModOfPower

```c#
/*
 * n: Number of digits of x
 * N: Number of clusters of x (i.e. n / 18)
 * p: Number of digits of pow
 * P: Number of clusters of pow (i.e. p / 18)
 * Total Complexity: O(N¹·⁵⁸(log₂(P))
 */
ModOfPower(x, pow, mod)
{
    if (x.IsZero) return BigZero                                                         // O(1)
    if (pow.IsZero) return BigOne                                                        // O(1)
    if (pow.IsOne) return x                                                              // O(1)
    (quotient, remainder) = pow.Divide(2)                                                // O(P)
    result = ModOfPower(x, quotient, mod)                                                // [★]
    result = result * result % mod                                                       // O(N¹·⁵⁸)
    if (remainder.IsZero) return result                                                  // O(1)
    else return result * (x % mod) % mod                                                 // O(N¹·⁵⁸)
}
/* [★]:
*       T(Recursive Code)     = T(P/2)
*       T(Non-Recursive Code) = P + 2N¹·⁵⁸
*       T(N) = T(Recursive Code) + T(Non-Recursive Code)
*       T(N) = T(P/2) + P + 2N¹·⁵⁸
*
*       Using Iterative Method:
*
*           T(P)   = T(P/2) +              P   + 2N¹·⁵⁸   -> [1]
*           T(P/2) = T(P/4) +        P/2       + 2N¹·⁵⁸
*           T(P)   = T(P/4) +        P/2 + P   + 4N¹·⁵⁸   -> [2]
*           T(P/4) = T(P/8) +  P/4             + 2N¹·⁵⁸
*           T(P)   = T(P/8) +  P/4 + P/2 + P   + 6N¹·⁵⁸   -> [3]
*
*        ∴  General Form  : T(N) = T(P/2ᵏ) + ᵢ₌₀𝛴ᵏ(P/2ⁱ) + 2KN¹·⁵⁸      (where K is iteration number)
*           and Base Case : T(N) = 1
*
*       By Substitution:
*
*           T(P/2ᵏ) = O(1)
*           P/2ᵏ    = 1
*           2ᵏ      = P
*           K       = log₂(P)
*
*        ∴  T(N) = 1 + P[ (1-2)/(1-2⁽ˡᵒᵍ₂ᴾ⁾⁺¹) ] + 2N¹·⁵⁸ [ log₂(P) ]
*           T(N) = 1 + [ (-P)/(1-2P) ] + 2N¹·⁵⁸ [ log₂(P) ]
*           T(N) = O(N¹·⁵⁸(log₂(P))
*/
```
