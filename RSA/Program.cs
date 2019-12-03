using System;
using Type.BigInteger;

namespace RSA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var big = new BigInteger("999");
            Console.WriteLine(string.Join("\n", big.Clusters));
        }
    }
}
