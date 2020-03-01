using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Serialization.Json.Tests.Commands
{
    internal class FakeCommand : ICommand
    {
        public Guid Id { get; }
        public string Subject { get; }
        public Guid CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.Empty;
            Subject = Guid.Empty.ToString();
            Timestamp = DateTimeOffset.MaxValue;
            Version = 3;
        }
    }
}
