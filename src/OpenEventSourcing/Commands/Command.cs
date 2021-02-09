using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    public abstract class Command : ICommand
    {
        public CommandId Id { get; }
        public string Subject { get; }
        public CorrelationId? CorrelationId { get; }
        public int Version { get; }
        public Actor Actor { get; }
        public DateTimeOffset Timestamp { get; }

        public Command(string subject, CorrelationId? correlationId, int version, Actor actor)
        {
            Id = CommandId.New();
            Subject = subject;
            CorrelationId = correlationId;
            Version = version;
            Actor = actor;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
