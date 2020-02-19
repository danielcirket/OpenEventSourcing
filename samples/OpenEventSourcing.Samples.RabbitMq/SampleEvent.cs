using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.RabbitMq
{
    public class SampleEvent : Event
    {
        public SampleEvent(string subject, int version) 
            : base(subject, version)
        {
        }
    }
}
