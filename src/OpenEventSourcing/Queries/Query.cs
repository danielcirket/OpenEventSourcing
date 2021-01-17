using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    public abstract class Query<TQueryResult> : IQuery<TQueryResult>
    {
        public string Id { get; }
        public DateTimeOffset Timestamp { get; }
        public CorrelationId? CorrelationId { get; }
        public string UserId { get; }

        public Query(CorrelationId? correlationId, string userId)
        {
            Id = Guid.NewGuid().ToSequentialGuid().ToString();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = correlationId;
            UserId = userId;
        }
    }
}
