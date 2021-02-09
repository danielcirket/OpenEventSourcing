using System;

namespace OpenEventSourcing.Commands
{
    public readonly struct CommandId : IEquatable<CommandId>
    {
        internal string Value { get; }

        internal CommandId(string value)
        {
            Value = value;
        }

        public static CommandId New()
        {
            var value = Base64UrlIdGenerator.New();
            
            return new CommandId(value);
        }
        public static CommandId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new CommandId(value);
        }
        
        public bool Equals(CommandId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CommandId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(CommandId left, CommandId right) => left.Equals(right);
        public static bool operator !=(CommandId left, CommandId right) => !left.Equals(right);
        public static implicit operator string(CommandId id) => id.Value;
    }
}
