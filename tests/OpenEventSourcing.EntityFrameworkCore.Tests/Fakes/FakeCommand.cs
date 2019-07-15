using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeCommand : ICommand
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            AggregateId = Guid.NewGuid().ToSequentialGuid();
            CorrelationId = Guid.NewGuid().ToSequentialGuid();
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
