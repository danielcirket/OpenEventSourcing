using System;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeQuery : IQuery<FakeQueryResult>
    {
        public Guid Id { get; }
        public DateTimeOffset Timestamp { get; }
        public Guid CorrelationId { get; }
        public string UserId { get; }

        public FakeQuery()
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            Timestamp = DateTimeOffset.UtcNow;
            CorrelationId = Guid.NewGuid().ToSequentialGuid();
            UserId = null;
        }
    }
}
