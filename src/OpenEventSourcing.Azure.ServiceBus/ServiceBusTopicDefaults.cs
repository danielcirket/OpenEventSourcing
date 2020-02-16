using System;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public static class ServiceBusTopicDefaults
    {
        public static readonly TimeSpan TimeToLive = TimeSpan.MaxValue;
        public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan DeleteOnIdleAfter = TimeSpan.MaxValue;
    }
}
