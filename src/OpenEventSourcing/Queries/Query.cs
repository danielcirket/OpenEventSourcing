using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    public abstract class Query<TQueryResult> : IQuery<TQueryResult>
    {
        public string Id { get; }
        public DateTimeOffset Timestamp { get; }
        public string CorrelationId { get; }
        public string UserId { get; }

        public Query(string correlationId, string userId)
        {
            Id = Guid.NewGuid().ToSequentialGuid().ToString();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = correlationId;
            UserId = userId;
        }
    }
}
