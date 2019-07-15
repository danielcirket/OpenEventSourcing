using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace OpenEventSourcing.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientManager
    {
        Task<IReadOnlyList<ISubscriptionClient>> ConfigureAsync();
    }
}
