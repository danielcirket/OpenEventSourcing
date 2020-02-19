using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Events
{
    public interface IEvent
    {
        Guid Id { get; }
        string Subject { get; }
        Guid? CorrelationId { get; }
        Guid? CausationId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
        string UserId { get; }

        void UpdateFrom(ICommand command);
    }
}
