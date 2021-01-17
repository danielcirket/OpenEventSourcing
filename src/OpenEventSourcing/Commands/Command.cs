using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    public abstract class Command : ICommand
    {
        public string Id { get; }
        public string Subject { get; }
        public CorrelationId? CorrelationId { get; }
        public int Version { get; }
        public string UserId { get; }
        public DateTimeOffset Timestamp { get; }

        public Command(string subject, CorrelationId? correlationId, int version, string userId)
        {
            Id = Guid.NewGuid().ToSequentialGuid().ToString();
            Subject = subject;
            CorrelationId = correlationId;
            Version = version;
            UserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
