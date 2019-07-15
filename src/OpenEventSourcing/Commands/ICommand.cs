using System;

namespace OpenEventSourcing.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
        Guid AggregateId { get; }
        Guid CorrelationId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
        string UserId { get; }
    }
}
