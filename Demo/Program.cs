using System;
using Crypto.RSA;
using Crypto.RSA.Encoders;
using Crypto.RSA.Keys;
using Type.BigInteger;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Number Encryption Test
            const string message1 = "123456";
            var publicKey1 = new PublicKey(new BigInteger("17"), new BigInteger("3658315382137043"));
            var encryptedMessage = RSA.Encrypt(publicKey1, new BigInteger(message1));

            var privateKey = new PrivateKey(new BigInteger("3012726845747393"), new BigInteger("3658315382137043"));
            var decryptedMessage = RSA.Decrypt(privateKey, encryptedMessage);

            CaseLog("#1. Number Encryption", message1, publicKey1, encryptedMessage.ToString(), privateKey,
                decryptedMessage.ToString());

            // String Encryption Test
            const string message2 = "MMMMY";
            var publicKey2 = new PublicKey(new BigInteger("17"), new BigInteger("3658315382137043"));
            encryptedMessage = RSA.Encrypt(publicKey2, message2, AsciiEncoder.Instance);

            privateKey = new PrivateKey(new BigInteger("3012726845747393"), new BigInteger("3658315382137043"));
            var decryptedMessage2 = RSA.Decrypt(privateKey, encryptedMessage, AsciiEncoder.Instance);

            CaseLog("#2. String Encryption", message2, publicKey2, encryptedMessage.ToString(), privateKey,
                decryptedMessage2);


            // Generate key Test
            var generatedKey = new PublicKey(new BigInteger("17"), new BigInteger("3658315382137043"));

            LogTitle("#3. Generate key");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("        [PUBLIC KEY]");
            Console.ResetColor();
            Console.WriteLine($"        Exponent: {generatedKey.Exponent} | Modulus: {generatedKey.Modulus}");
            Console.WriteLine();
        }

        private static void CaseLog(string title, string message, Key publicKey, string encryptedMessage,
            Key privateKey, string decryptedMessage)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"        {title}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("        [MESSAGE]");
            Console.ResetColor();
            Console.WriteLine($"           {message}");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("        [PUBLIC KEY]");
            Console.ResetColor();

            Console.WriteLine($"        Exponent: {publicKey.Exponent} | Modulus: {publicKey.Modulus}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("        [ENCRYPTION]");
            Console.ResetColor();

            Console.WriteLine($"        Encrypted Message: {encryptedMessage}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("        [PRIVATE KEY]");
            Console.ResetColor();

            Console.WriteLine($"       Exponent: {privateKey.Exponent} | Modulus: {privateKey.Modulus}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("        [DECRYPTION]");
            Console.ResetColor();

            Console.WriteLine($"        Decrypted Message: {decryptedMessage}");
            Console.WriteLine();
        }

        private static void LogRecord(string key, string value, ConsoleColor color)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.Write($"        [{key}]");
            Console.ResetColor();
            Console.WriteLine($"           {value}");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"        {key}");
            Console.WriteLine();

        }

        private static void LogTitle(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"        {title}");
            Console.WriteLine();
        }


    }
}