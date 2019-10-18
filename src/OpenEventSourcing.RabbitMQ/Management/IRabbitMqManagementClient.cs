﻿using System;
using System.Threading.Tasks;

namespace OpenEventSourcing.RabbitMQ.Management
{
    public interface IRabbitMqManagementClient : IDisposable
    {
        Task CreateExchangeAsync(string name, string exchangeType, bool durable = true, bool autoDelete = false);
        Task CreateQueueAsync(string name, bool durable = true, bool autoDelete = false);
        Task CreateSubscriptionAsync(string filter, string queue, string exchange);
        Task<bool> ExchangeExistsAsync(string name);
        Task<bool> QueueExistsAsync(string name);
        Task RemoveExchangeAsync(string name);
        Task RemoveQueueAsync(string name);
        Task RemoveSubscriptionAsync(string filter, string queue, string exchange);
    }
}
