using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Serialization.Json.Tests.Commands
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
            Timestamp = DateTimeOffset.MaxValue;
            Version = 3;
        }
    }
}
