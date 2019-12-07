using System;
using Type.BigInteger;

namespace RSA
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var big = new BigInteger("-500");
            var result = big.Subtract(new BigInteger("12"));
            
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Size: {result.Size}");
            Console.WriteLine($"Clusters: {result.ClustersLength}");
            Console.WriteLine($"As Expected: {result.ToString() == "-512"}");
        }
    }
}
