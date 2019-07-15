using Microsoft.Azure.ServiceBus;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    public interface ITopicClientFactory
    {
        ITopicClient Create();
    }
}
