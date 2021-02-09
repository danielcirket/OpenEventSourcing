using System;

namespace OpenEventSourcing.Events
{
    public readonly struct EventId : IEquatable<EventId>
    {
        internal string Value { get; }

        internal EventId(string value)
        {
            Value = value;
        }

        public static EventId New()
        {
            var value = Base64UrlIdGenerator.New();
            
            return new EventId(value);
        }
        public static EventId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new EventId(value);
        }
        
        public bool Equals(EventId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EventId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(EventId left, EventId right) => left.Equals(right);
        public static bool operator !=(EventId left, EventId right) => !left.Equals(right);
        public static implicit operator string(EventId id) => id.Value;
    }
}
