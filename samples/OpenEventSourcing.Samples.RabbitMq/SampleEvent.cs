using System;
using System.Collections.Generic;
using System.Text;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.RabbitMq
{
    public class SampleEvent : Event
    {
        public SampleEvent(Guid aggregateId, int version) 
            : base(aggregateId, version)
        {
        }
    }
}
