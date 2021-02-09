using System;

namespace OpenEventSourcing
{
    public readonly struct Actor : IEquatable<Actor>
    {
        internal string Value { get; }

        internal Actor(string value)
        {
            Value = value;
        }
        
        public static Actor From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Actor(value);
        }
        
        public bool Equals(Actor other) => Value == other.Value;
        public override bool Equals(object obj) => obj is Actor other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(Actor left, Actor right) => left.Equals(right);
        public static bool operator !=(Actor left, Actor right) => !left.Equals(right);
        public static implicit operator string(Actor id) => id.Value;
    }
}
