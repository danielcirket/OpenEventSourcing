using System;

namespace OpenEventSourcing
{
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        internal string Value { get; }

        internal CorrelationId(string value)
        {
            Value = value;
        }

        public static CorrelationId New()
        {
            var value = Base64UrlIdGenerator.New();
            
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
