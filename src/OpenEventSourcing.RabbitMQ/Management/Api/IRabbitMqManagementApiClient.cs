using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenEventSourcing.RabbitMQ.Management.Api
{
    public interface IRabbitMqManagementApiClient
    {
        Task<IEnumerable<RabbitMqBinding>> RetrieveSubscriptionsAsync(string queue);
    }
}
