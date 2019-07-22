using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Serialization.Json.Tests.Events
{
    internal class FakeEvent : IEvent
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public Guid? CorrelationId { get; }
        public Guid? CausationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public void UpdateFrom(ICommand command)
        {
            throw new NotImplementedException();
        }

        public FakeEvent()
        {
            Id = Guid.Empty;
            AggregateId = Guid.Empty;
            Timestamp = DateTimeOffset.MaxValue;
            Version = 2;
        }
    }
}
