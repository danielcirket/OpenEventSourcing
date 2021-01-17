using System;
using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;

namespace OpenEventSourcing
{
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        internal string Value { get; }

        internal CorrelationId(string value)
        {
            Value = value;
        }

        private static string Base64UrlEncode(ReadOnlySpan<byte> input)
        {
            if (input.IsEmpty)
                return string.Empty;

            var bufferSize = GetArraySizeRequiredToEncode(input.Length);
            char[] bufferToReturnToPool = null;
            
            Span<char> buffer = bufferSize <= 128
                ? stackalloc char[bufferSize]
                : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(bufferSize);

            var base64CharCount = Base64UrlEncode(input, buffer);

            var result = new string(buffer.Slice(0, base64CharCount));
            
            if (bufferToReturnToPool != null)
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);

            return result;
        }
        private static int Base64UrlEncode(ReadOnlySpan<byte> input, Span<char> output)
        {
            Debug.Assert(output.Length >= GetArraySizeRequiredToEncode(input.Length));
            
            if (input.IsEmpty)
                return 0;
            
            // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.
            Convert.TryToBase64Chars(input, output, out var charsWritten);
            
            // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
            for (var i = 0; i < charsWritten; i++)
            {
                var ch = output[i];
                if (ch == '+')
                {
                    output[i] = '-';
                }
                else if (ch == '/')
                {
                    output[i] = '_';
                }
                else if (ch == '=')
                {
                    // We've reached a padding character; truncate the remainder.
                    return i;
                }
            }
            
            return charsWritten;
        }
        private static int GetArraySizeRequiredToEncode(int count)
        {
            var numWholeOrPartialInputBlocks = checked(count + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }

        public static CorrelationId New()
        {
            Span<byte> buffer = stackalloc byte[16];
            // Generate the id with RNGCrypto because we want a cryptographically random id, which GUID is not
            RandomNumberGenerator.Fill(buffer);

            var value = Base64UrlEncode(buffer);
            
            return new CorrelationId(value);
        }
        public static CorrelationId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new CorrelationId(value);
        }
        
        public bool Equals(CorrelationId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CorrelationId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(CorrelationId left, CorrelationId right) => left.Equals(right);
        public static bool operator !=(CorrelationId left, CorrelationId right) => !left.Equals(right);
        public static implicit operator string(CorrelationId id) => id.Value;
    }
}
