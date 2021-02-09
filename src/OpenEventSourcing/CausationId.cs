using System;

namespace OpenEventSourcing
{
    public readonly struct CausationId : IEquatable<CausationId>
    {
        internal string Value { get; }

        internal CausationId(string value)
        {
            Value = value;
        }

        public static CausationId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new CausationId(value);
        }
        
        public bool Equals(CausationId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CausationId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(CausationId left, CausationId right) => left.Equals(right);
        public static bool operator !=(CausationId left, CausationId right) => !left.Equals(right);
        public static implicit operator string(CausationId id) => id.Value;
    }
}
