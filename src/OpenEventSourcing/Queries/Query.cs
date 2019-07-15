using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    public abstract class Query<TQueryResult> : IQuery<TQueryResult>
    {
        public Guid Id { get; }
        public DateTimeOffset Timestamp { get; }
        public Guid CorrelationId { get; }
        public string UserId { get; }

        public Query(Guid correlationId, string userId)
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = correlationId;
            UserId = userId;
        }
    }
}
