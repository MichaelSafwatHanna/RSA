using System;
using Type.BigInteger;

namespace RSA
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var bigInt = new BigInteger("123456789");
            var actual = bigInt.ShiftLeft(10);
            var expect = new BigInteger("1234567890000000000");

            Console.WriteLine();
            Console.WriteLine($"    [BigInt]     {bigInt} | Size: {bigInt.Size} | Clusters: {bigInt.ClustersLength}");
            Console.WriteLine($"    [Actual]     {actual} | Size: {actual.Size} | Clusters: {actual.ClustersLength}");
            Console.WriteLine($"    [Expect]     {expect} | Size: {expect.Size} | Clusters: {expect.ClustersLength}");
            Console.WriteLine($"    [Status]     {actual.Equals(expect)}");
            Console.WriteLine();
        }
    }
}
