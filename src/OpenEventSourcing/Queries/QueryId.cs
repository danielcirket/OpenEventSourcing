using System;

namespace OpenEventSourcing.Queries
{
    public readonly struct QueryId : IEquatable<QueryId>
    {
        internal string Value { get; }

        internal QueryId(string value)
        {
            Value = value;
        }

        public static QueryId New()
        {
            var value = Base64UrlIdGenerator.New();
            
            return new QueryId(value);
        }
        public static QueryId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new QueryId(value);
        }
        
        public bool Equals(QueryId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is QueryId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(QueryId left, QueryId right) => left.Equals(right);
        public static bool operator !=(QueryId left, QueryId right) => !left.Equals(right);
        public static implicit operator string(QueryId id) => id.Value;
    }
}
