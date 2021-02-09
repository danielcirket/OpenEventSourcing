using System;

namespace OpenEventSourcing.Commands
{
    public interface ICommand
    {
        CommandId Id { get; }
        string Subject { get; }
        CorrelationId? CorrelationId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
        Actor Actor { get; }
    }
}
