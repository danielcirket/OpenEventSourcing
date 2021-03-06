﻿using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.RabbitMQ.Connections
{
    public interface IRabbitMqConnectionFactory
    {
        Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}
