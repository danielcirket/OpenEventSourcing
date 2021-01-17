using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeCommand : ICommand
    {
        public string Id { get; }
        public string Subject { get; }
        public CorrelationId? CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.NewGuid().ToSequentialGuid().ToString();
            Subject = Guid.NewGuid().ToSequentialGuid().ToString();
            CorrelationId = OpenEventSourcing.CorrelationId.New();
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
