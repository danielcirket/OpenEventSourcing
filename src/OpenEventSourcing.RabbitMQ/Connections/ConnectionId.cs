using System;
using System.Security.Cryptography;

namespace OpenEventSourcing.RabbitMQ.Connections
{
    public class ConnectionId
    {
        private static readonly RNGCryptoServiceProvider _keyGenerator = new RNGCryptoServiceProvider();

        public string Value { get; }

        internal ConnectionId()
        {
            var input = new byte[16];

            // Generate the id with RNGCrypto because we want a cryptographically random id, which GUID is not
            _keyGenerator.GetBytes(input);

            var output = new char[GetArraySizeRequiredToEncode(input.Length)];
            var length = Base64UrlEncode(input, 0, output, 0, input.Length);

            Value = new string(output, startIndex: 0, length: length);
        }
        private static int GetArraySizeRequiredToEncode(int count)
        {
            var numWholeOrPartialInputBlocks = checked(count + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }
        private static int Base64UrlEncode(byte[] input, int inOffset, char[] output, int outOffset, int count)
        {
            var result = Convert.ToBase64CharArray(input, inOffset, count, output, outOffset);

            for (int i = outOffset; i - outOffset < result; i++)
            {
                var ch = output[i];

                if (ch == '+')
                    output[i] = '-';

                if (ch == '/')
                    output[i] = '_';

                if (ch == '=')
                    return i - outOffset;
            }

            return result;
        }

        public static ConnectionId New()
        {
            return new ConnectionId();
        }

        public static implicit operator string(ConnectionId id) => id.Value;
    }
}
