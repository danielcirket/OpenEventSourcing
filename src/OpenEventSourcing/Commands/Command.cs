using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    public abstract class Command : ICommand
    {
        public Guid Id { get; }
        public string Subject { get; }
        public Guid CorrelationId { get; }
        public int Version { get; }
        public string UserId { get; }
        public DateTimeOffset Timestamp { get; }

        public Command(string subject, Guid correlationId, int version, string userId)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));

            Id = Guid.NewGuid().ToSequentialGuid();
            Subject = subject;
            CorrelationId = correlationId;
            Version = version;
            UserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
