using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Events
{
    public abstract class Event : IEvent
    {
        public EventId Id { get; protected set; }
        public string Subject { get; protected set; }
        public DateTimeOffset Timestamp { get; protected set; }
        public int Version { get; protected set; }

        public Event(string subject, int version)
        {
            Id = EventId.New();
            Subject = subject;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
        }
    }
}
