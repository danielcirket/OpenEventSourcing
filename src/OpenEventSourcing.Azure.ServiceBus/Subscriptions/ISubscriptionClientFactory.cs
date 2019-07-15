using Microsoft.Azure.ServiceBus;

namespace OpenEventSourcing.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create(ServiceBusSubscription subscription);
    }
}
