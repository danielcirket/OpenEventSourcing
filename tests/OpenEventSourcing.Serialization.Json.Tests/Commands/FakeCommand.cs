using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Serialization.Json.Tests.Commands
{
    internal class FakeCommand : ICommand
    {
        public CommandId Id { get; }
        public string Subject { get; }
        public CorrelationId? CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public Actor Actor { get; }

        public FakeCommand()
        {
            Id = CommandId.From(Guid.Empty.ToString());
            Subject = Guid.Empty.ToString();
            CorrelationId = OpenEventSourcing.CorrelationId.From(Guid.Empty.ToString());
            Timestamp = DateTimeOffset.MaxValue;
            Version = 3;
        }
    }
}
