using System;

namespace OpenEventSourcing.Queries
{
    public interface IQuery<out TQueryResult>
    {
        Guid Id { get; }
        DateTimeOffset Timestamp { get; }
        Guid CorrelationId { get; }
        string UserId { get; }
    }
}
