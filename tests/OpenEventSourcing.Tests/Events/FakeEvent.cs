using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Tests.Events
{
    internal class FakeEvent : Event
    {
        public FakeEvent() : base(Guid.NewGuid(), 1) { }
    }
}
