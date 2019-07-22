using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Serialization.Json.Tests.Queries
{
    internal class FakeQuery : IQuery<FakeQueryResult>
    {
        public Guid Id { get; }
        public DateTimeOffset Timestamp { get; }
        public Guid CorrelationId { get; }
        public string UserId { get; }

        public FakeQuery()
        {
            Id = Guid.Empty;
            Timestamp = DateTimeOffset.MinValue;
            CorrelationId = Guid.Empty;
        }
    }
}
