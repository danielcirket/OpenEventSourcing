using System.Threading.Tasks;

namespace OpenEventSourcing.RabbitMQ.Subscriptions
{
    public interface ISubscriptionManager
    {
        Task ConfigureAsync();
    }
}
