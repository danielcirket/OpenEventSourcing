using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.AzureServiceBus
{
    public class SampleEvent : Event
    {
        public SampleEvent(Guid aggregateId, int version) 
            : base(aggregateId, version)
        {
        }
    }
}
