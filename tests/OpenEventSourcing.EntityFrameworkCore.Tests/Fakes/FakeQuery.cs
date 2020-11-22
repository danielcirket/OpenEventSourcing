using System;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeQuery : IQuery<FakeQueryResult>
    {
        public string Id { get; }
        public DateTimeOffset Timestamp { get; }
        public string CorrelationId { get; }
        public string UserId { get; }

        public FakeQuery()
        {
            Id = Guid.NewGuid().ToSequentialGuid().ToString();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = Guid.NewGuid().ToSequentialGuid().ToString();
            UserId = null;
        }
    }
}
