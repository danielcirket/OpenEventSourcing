using System;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public static class ServiceBusSubscriptionDefaults
    {
        public static readonly int MaxDeliveryCount = 10;
        public static readonly bool UseDeadLetterOnExpiration = false;
        public static readonly TimeSpan TimeToLive = TimeSpan.MaxValue;
        public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DeleteOnIdleAfter = TimeSpan.MaxValue;
    }
}
