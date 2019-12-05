using System;
using Type.BigInteger;

namespace RSA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var big = new BigInteger("1000000000000000001");
            var result = big.Add(new BigInteger("1"));
            
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Size: {result.Size}");
            Console.WriteLine($"Clusters: {result.ClustersLength}");
            Console.WriteLine($"As Expected: {result.ToString() == "1000000000000000002"}");
        }
    }
}
