using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class SubscriptionAlreadyExistsException : Exception
    {
        public string SubscriptionName { get; }

        public SubscriptionAlreadyExistsException(string name)
            : base($"A subscription with name '{name}' already exists.")
        {
            SubscriptionName = name;
        }
    }
}
