using System;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeQuery : IQuery<FakeQueryResult>
    {
        public QueryId Id { get; }
        public DateTimeOffset Timestamp { get; }
        public CorrelationId? CorrelationId { get; }
        public Actor Actor { get; }

        public FakeQuery()
        {
            Id = QueryId.New();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = OpenEventSourcing.CorrelationId.From(Guid.NewGuid().ToString());
            Actor = Actor.From(nameof(FakeQuery));
        }
    }
}
