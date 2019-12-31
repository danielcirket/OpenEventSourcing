using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    public interface ITopicClientFactory
    {
        Task<ITopicClient> CreateAsync();
    }
}
