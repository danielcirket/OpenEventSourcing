using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeEvent : Event
    {
        public FakeEvent(string subject, int version)
            : base(subject, version)
        {
        }
    }
}
