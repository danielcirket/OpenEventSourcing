using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeCommand : ICommand
    {
        public string Id { get; }
        public string Subject { get; }
        public string CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.Empty.ToString();
            Subject = Guid.NewGuid().ToString();
            Timestamp = DateTimeOffset.MaxValue;
            Version = 1;
        }
    }
}
