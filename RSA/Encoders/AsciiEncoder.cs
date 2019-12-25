using System;

namespace Crypto.RSA.Encoders
{
    public class AsciiEncoder : IEncoder
    {
        private const int MaxAsciiLength = 3;

        public static AsciiEncoder Instance { get; } = new AsciiEncoder();

        private AsciiEncoder() { }

        public string GetBytes(string str)
        {
            var length = str.Length * MaxAsciiLength;
            var result = new char[length];

            for (var i = 0; i < length; i += MaxAsciiLength)
            {
                int ascii = str[i / MaxAsciiLength];
                var asciiStr = ascii.ToString();
                var offset = MaxAsciiLength - asciiStr.Length;

                for (var j = 0; j < MaxAsciiLength; j++)
                {
                    result[i + j] = j < offset ? '0' : asciiStr[j - offset];
                }
            }

            return new string(result);
        }

        public string GetString(string bytes)
        {
            var length = (int)Math.Ceiling(bytes.Length / (double)MaxAsciiLength);
            var result = new char[length];

            for (var i = bytes.Length - MaxAsciiLength; i >= 0; i -= MaxAsciiLength)
            {
                result[length - 1] = (char)Convert.ToInt32(bytes.Substring(i, MaxAsciiLength));
                length--;
            }

            var remainder = bytes.Length % MaxAsciiLength;
            if (remainder != 0)
            {
                result[0] = (char)Convert.ToInt32(bytes.Substring(0, remainder));
            }

            return new string(result);
        }

    }
}