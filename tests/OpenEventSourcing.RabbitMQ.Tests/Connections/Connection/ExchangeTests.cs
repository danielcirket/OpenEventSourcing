﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class ExchangeTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ExchangeTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                            .UseExchange("test-exchange");
                    });

            ServiceProvider = services.BuildServiceProvider();
        }

        [IntegrationTest]
        public void WhenCreateExchangeAsyncCalledWithNonExistentExchangeThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            verify.Should().Throw<ExchangeAlreadyExistsException>()
                .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenCreateExchangeAsyncCalledWithExistingExchangeThenShouldThrowExchangeAlreadyExistsException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            act.Should().Throw<ExchangeAlreadyExistsException>()
                .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithNonExistentExchangeThenShouldThrowExhangeNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.RemoveExchangeAsync(name: exchangeName);
            };

            act.Should().Throw<ExchangeNotFoundException>()
               .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithExistingExchangeThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.RemoveExchangeAsync(name: exchangeName);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.RemoveExchangeAsync(name: exchangeName);
            };

            verify.Should().Throw<ExchangeNotFoundException>()
                .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithNonExistentExchangeThenShouldReturnFalse()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                var result = await connection.ExchangeExistsAsync(name: exchangeName);

                result.Should().BeFalse();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithExistingExchangeThenShouldReturnTrue()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                var result = await connection.ExchangeExistsAsync(name: exchangeName);

                result.Should().BeTrue();
            };

            verify.Should().NotThrow();
        }
    }
}