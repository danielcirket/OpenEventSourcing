using System;

namespace OpenEventSourcing.Queries
{
    public interface IQuery<out TQueryResult>
    {
        string Id { get; }
        DateTimeOffset Timestamp { get; }
        string CorrelationId { get; }
        string UserId { get; }
    }
}
