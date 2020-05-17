using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeCommand : ICommand
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.Empty;
            AggregateId = Guid.NewGuid();
            Timestamp = DateTimeOffset.MaxValue;
            Version = 1;
        }
    }
}
