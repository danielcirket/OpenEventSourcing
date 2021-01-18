using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Serialization.Json.Tests.Events
{
    internal class FakeEvent : IEvent
    {
        public EventId Id { get; }
        public string Subject { get; }
        public string CorrelationId { get; }
        public string CausationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public void UpdateFrom(ICommand command)
        {
            throw new NotImplementedException();
        }

        public FakeEvent()
        {
            Id = EventId.From(Guid.Empty.ToString());
            Subject = Guid.Empty.ToString();
            Timestamp = DateTimeOffset.MaxValue;
            Version = 2;
        }
    }
}
