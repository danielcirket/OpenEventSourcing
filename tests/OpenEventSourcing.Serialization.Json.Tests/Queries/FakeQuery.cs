using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Serialization.Json.Tests.Queries
{
    internal class FakeQuery : IQuery<bool>
    {
        public QueryId Id { get; }
        public DateTimeOffset Timestamp { get; }
        public CorrelationId? CorrelationId { get; }
        public Actor Actor { get; }

        public FakeQuery()
        {
            Id = QueryId.From(Guid.Empty.ToString());
            Timestamp = DateTimeOffset.MinValue;
            CorrelationId = OpenEventSourcing.CorrelationId.From(Guid.Empty.ToString());
        }
    }
}
