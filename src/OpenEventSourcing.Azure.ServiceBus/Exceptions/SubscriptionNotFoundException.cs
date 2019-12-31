using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class SubscriptionNotFoundException : Exception
    {
        public string SubscriptionName { get; }

        public SubscriptionNotFoundException(string name)
            : base($"A subscription with name '{name}' could not be found.")
        {
            SubscriptionName = name;
        }
    }
}
