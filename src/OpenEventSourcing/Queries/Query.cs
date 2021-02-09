using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    public abstract class Query<TQueryResult> : IQuery<TQueryResult>
    {
        public QueryId Id { get; }
        public DateTimeOffset Timestamp { get; }
        public CorrelationId? CorrelationId { get; }
        public Actor Actor { get; }

        public Query(CorrelationId? correlationId, Actor actor)
        {
            Id = QueryId.New();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = correlationId;
            Actor = actor;
        }
    }
}
