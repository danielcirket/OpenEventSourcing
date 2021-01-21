using System;

namespace OpenEventSourcing.Events
{
    public readonly struct StreamId : IEquatable<StreamId>
    {
        internal string Value { get; }

        internal StreamId(string value)
        {
            Value = value;
        }

        public static StreamId New()
        {
            var value = Base64UrlIdGenerator.New();
            
            return new StreamId(value);
        }
        public static StreamId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new StreamId(value);
        }
        
        public bool Equals(StreamId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is StreamId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(StreamId left, StreamId right) => left.Equals(right);
        public static bool operator !=(StreamId left, StreamId right) => !left.Equals(right);
        public static implicit operator string(StreamId id) => id.Value;
    }
}
