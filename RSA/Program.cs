using System;
using Type.BigInteger;

namespace RSA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var big = new BigInteger("10000000000000000001");
            var result = big.Add(new BigInteger("1"));
            
            Console.WriteLine(result.ToString());
        }
    }
}
