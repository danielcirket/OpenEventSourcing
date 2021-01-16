using System;

namespace OpenEventSourcing.Commands
{
    public interface ICommand
    {
        string Id { get; }
        string Subject { get; }
        string CorrelationId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
        string UserId { get; }
    }
}
