using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeEvent : Event
    {
        public FakeEvent(Guid aggregateId, int version)
            : base(aggregateId, version)
        {
        }
    }
}
