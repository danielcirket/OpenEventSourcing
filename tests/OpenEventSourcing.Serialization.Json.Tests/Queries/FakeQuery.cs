using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Serialization.Json.Tests.Queries
{
    internal class FakeQuery : IQuery<FakeQueryResult>
    {
        public string Id { get; }
        public DateTimeOffset Timestamp { get; }
        public string CorrelationId { get; }
        public string UserId { get; }

        public FakeQuery()
        {
            Id = Guid.Empty.ToString();
            Timestamp = DateTimeOffset.MinValue;
            CorrelationId = Guid.Empty.ToString();
        }
    }
}
