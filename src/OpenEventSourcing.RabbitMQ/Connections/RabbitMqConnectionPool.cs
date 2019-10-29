using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Connections
{
    internal sealed class RabbitMqConnectionPool
    {
        private readonly ConcurrentQueue<RabbitMqConnection> _availableConnections;
        private readonly ILogger<RabbitMqConnectionPool> _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly ConcurrentDictionary<Uri, ConnectionFactory> _connectionFactories;
        private readonly object _lock = new object();

        public int AvailableConnections => _availableConnections.Count;

        public RabbitMqConnectionPool(ILogger<RabbitMqConnectionPool> logger,
                                      IOptions<RabbitMqOptions> options)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger = logger;
            _options = options;
            _availableConnections = new ConcurrentQueue<RabbitMqConnection>();
            _connectionFactories = new ConcurrentDictionary<Uri, ConnectionFactory>();
        }

        public IRabbitMqConnection GetConnection()
        {
            lock (_lock)
            {
                if (_availableConnections.TryDequeue(out var connection) && connection.IsOpen)
                    return connection;

                connection?.Dispose();

                var factory = CreateConnectionFactory();
                var connectionId = ConnectionId.New();

                connection = new RabbitMqConnection(connectionId, factory.CreateConnection(connectionId), this, _options);

                return connection;
            }
        }
        public void ReturnConnection(RabbitMqConnection connection, bool reRegisterForFinalization)
        {
            lock (_lock)
            {
                if (connection.IsOpen)
                {
                    if (reRegisterForFinalization)
                        GC.ReRegisterForFinalize(connection);

                    connection.Reset();

                    _availableConnections.Enqueue(connection);
                    return;
                }

                connection.UnderlyingConnection?.Dispose();
            }
        }

        private ConnectionFactory CreateConnectionFactory()
        {
            var uri = new Uri(_options.Value.ConnectionString);

            if (_connectionFactories.TryGetValue(uri, out var factory))
                return factory;

            _logger.LogDebug($"Creating new {nameof(RabbitMqConnectionFactory)} for '{uri.Host}'");

            factory = new ConnectionFactory
            {
                Uri = uri,
                UseBackgroundThreadsForIO = true,
                TopologyRecoveryEnabled = true,
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true,
            };

            _connectionFactories.TryAdd(uri, factory);

            return factory;
        }
    }
}
