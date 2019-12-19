using System;
using Type.BigInteger;

namespace Crypto.RSA
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var input = RSA.Encoder.GetBytes("Zero Based");
            var n = new BigInteger("47594980475625417724408267823112764463863576918685226363032787239910118740004860624166859668486833021538759738968887527");
            var e = new BigInteger("17");
            var d = new BigInteger("22397637870882549517368596622641300924171095020557753582603446902846197377658196974714575577237681892436409853219169457");

            BigInteger enc = RSA.Encrypt(n, e, new BigInteger(input));
            BigInteger dec = RSA.Decrypt(n, d, enc);


            string output = RSA.Encoder.GetString(dec.ToString());

            Console.WriteLine("Encrypted : " + enc);
            Console.WriteLine("You Entered : " + output);

        }
    }
}