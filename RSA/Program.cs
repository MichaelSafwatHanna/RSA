using System;
using Type.BigInteger;

namespace RSA
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var bigInt = new BigInteger("123456789123456789987654321987654321123456789123456789");
            bigInt.SplitClusters(0, out var lower, out var upper);
            var expectLower = new BigInteger("0");
            var expectUpper = new BigInteger("123456789123456789987654321987654321123456789123456789");



            Console.WriteLine();
            Console.WriteLine($"    [BigInt]     {bigInt} | Length: {bigInt.Length} | Clusters: {bigInt.ClustersLength}");
            Console.WriteLine($"    [Actual]     {lower} | Length: {lower.Length} | Clusters: {lower.ClustersLength}");
            Console.WriteLine($"    [Expect]     {expectLower} | Length: {expectLower.Length} | Clusters: {expectLower.ClustersLength}");
            Console.WriteLine($"    [Status]     {lower.Equals(expectLower)}");
            Console.WriteLine();


            Console.WriteLine();
            Console.WriteLine($"    [BigInt]     {bigInt} | Length: {bigInt.Length} | Clusters: {bigInt.ClustersLength}");
            Console.WriteLine($"    [Actual]     {upper} | Length: {upper.Length} | Clusters: {upper.ClustersLength}");
            Console.WriteLine($"    [Expect]     {expectUpper} | Length: {expectUpper.Length} | Clusters: {expectUpper.ClustersLength}");
            Console.WriteLine($"    [Status]     {upper.Equals(expectUpper)}");
            Console.WriteLine();
        }
    }
}
