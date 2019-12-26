using System;
using System.IO;
using Crypto.RSA;
using Crypto.RSA.Keys;
using Type.BigInteger;

namespace Demo
{
    internal class Program
    {
        private const string SampleInputFile = @"../../../Cases/SampleRSA.txt";
        private const string SampleOutputFile = @"../../../Output/SampleOutput.txt";
        private const string CompleteInputFile = @"../../../Cases/TestRSA.txt";
        private const string CompleteOutputFile = @"../../../Output/CompleteOutput.txt";

        private static void Main(string[] args)
        {
            SampleTest();
            CompleteTest();
        }

        private static void SampleTest()
        {
            var inFileStream = new FileStream(SampleInputFile, FileMode.Open);
            var inStreamReader = new StreamReader(inFileStream);
            var outFileStream = new FileStream(SampleOutputFile, FileMode.Create);
            var outStreamWriter = new StreamWriter(outFileStream);

            var cases = int.Parse(inStreamReader.ReadLine());
            for (var i = 0; i < cases; i++)
            {
                var n = inStreamReader.ReadLine();
                var e = inStreamReader.ReadLine();
                var m = inStreamReader.ReadLine();
                var eFlag = inStreamReader.ReadLine();

                if (eFlag == "0")
                {
                    var ticksBefore = Environment.TickCount;
                    var actual = EncryptTest(n, e, m);
                    var ticksAfter = Environment.TickCount;
                    outStreamWriter.WriteLine(actual);
                    var totalMs = ticksAfter - ticksBefore;
                    outStreamWriter.WriteLine($"[{totalMs / 1000:00}.{totalMs % 1000} s]");
                }
                else
                {
                    var ticksBefore = Environment.TickCount;
                    var actual = DecryptTest(n, e, m);
                    var ticksAfter = Environment.TickCount;
                    outStreamWriter.WriteLine(actual);
                    var totalMs = ticksAfter - ticksBefore;
                    outStreamWriter.WriteLine($"[{totalMs / 1000:00}.{totalMs % 1000} s]");
                }
            }

            inStreamReader.Close();
            inFileStream.Close();
            outStreamWriter.Close();
            outFileStream.Close();
        }

        private static void CompleteTest()
        {
            var inFileStream = new FileStream(CompleteInputFile, FileMode.Open);
            var inStreamReader = new StreamReader(inFileStream);
            var outFileStream = new FileStream(CompleteOutputFile, FileMode.Create);
            var outStreamWriter = new StreamWriter(outFileStream);

            var cases = int.Parse(inStreamReader.ReadLine());
            for (var i = 0; i < cases; i++)
            {
                var n = inStreamReader.ReadLine();
                var e = inStreamReader.ReadLine();
                var m = inStreamReader.ReadLine();
                var eFlag = inStreamReader.ReadLine();

                if (eFlag == "0")
                {
                    var ticksBefore = Environment.TickCount;
                    var actual = EncryptTest(n, e, m);
                    var ticksAfter = Environment.TickCount;
                    outStreamWriter.WriteLine(actual);
                    var totalMs = ticksAfter - ticksBefore;
                    outStreamWriter.WriteLine($"[{totalMs / 1000:00}.{totalMs % 1000} s]");
                }
                else
                {
                    var ticksBefore = Environment.TickCount;
                    var actual = DecryptTest(n, e, m);
                    var ticksAfter = Environment.TickCount;
                    outStreamWriter.WriteLine(actual);
                    var totalMs = ticksAfter - ticksBefore;
                    outStreamWriter.WriteLine($"[{totalMs / 1000:00}.{totalMs % 1000} s]");
                }
            }

            inStreamReader.Close();
            inFileStream.Close();
            outStreamWriter.Close();
            outFileStream.Close();
        }

        private static BigInteger EncryptTest(string n, string e, string m)
        {
            var key = new PublicKey(new BigInteger(e), new BigInteger(n));
            var message = new BigInteger(m);
            return RSA.Encrypt(key, message);
        }

        private static BigInteger DecryptTest(string n, string d, string em)
        {
            var key = new PrivateKey(new BigInteger(d), new BigInteger(n));
            var encryptedMessage = new BigInteger(em);
            return RSA.Decrypt(key, encryptedMessage);
        }
    }
}