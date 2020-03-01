using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeCommand : ICommand
    {
        public Guid Id { get; }
        public string Subject { get; }
        public Guid CorrelationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public FakeCommand()
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            Subject = Guid.NewGuid().ToString();
            CorrelationId = Guid.NewGuid().ToSequentialGuid();
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
