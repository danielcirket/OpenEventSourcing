using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenEventSourcing.RabbitMQ.Subscriptions
{
    public interface ISubscriptionManager
    {
        Task ConfigureAsync();
    }
}
