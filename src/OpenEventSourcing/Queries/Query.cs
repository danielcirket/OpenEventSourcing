using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    public abstract class Query<TQueryResult> : IQuery<TQueryResult>
    {
        public QueryId Id { get; }
        public DateTimeOffset Timestamp { get; }
        public CorrelationId? CorrelationId { get; }
        public string UserId { get; }

        public Query(CorrelationId? correlationId, string userId)
        {
            Id = QueryId.New();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = correlationId;
            UserId = userId;
        }
    }
}
