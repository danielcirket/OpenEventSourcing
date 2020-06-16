using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;
using OpenEventSourcing.Tests.Fakes;
using Xunit;

namespace OpenEventSourcing.Tests.Queries.QueryDispatcher
{
    public class DispatchAsync
    {
        [Fact]
        public void WhenQueryIsNullThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddQueries()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            FakeQuery query = null;

            Func<Task> act = () => dispatcher.DispatchAsync(query);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(query));
        }
        [Fact]
        public void WhenNoHandlerRegisteredForQueryThenShouldThrowInvalidOperationException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddQueries()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            var command = new FakeQuery();

            Func<Task> act = () => dispatcher.DispatchAsync(command);

            act.Should().Throw<InvalidOperationException>()
                .And.Message.Should().Be($"No query handler for type '{typeof(FakeQuery).FriendlyName()}' has been registered.");
        }
        [Fact]
        public void WhenHandlerRegisteredForQueryThenShouldDispatchQueryToHandler()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddQueries()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            var handler = serviceProvider.GetRequiredService<IQueryHandler<FakeDispatchableQuery, bool>>();
            var query = new FakeDispatchableQuery();

            Func<Task> act = () => dispatcher.DispatchAsync(query);

            act.Should().NotThrow();

            ((FakeDispatchableQueryHandler)handler).Handled.Should().Be(1);
        }
        [Fact]
        public void WhenCancellationRequestedThenShouldThrowOperationCancelledException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddQueries()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            var handler = serviceProvider.GetRequiredService<IQueryHandler<FakeDispatchableQuery, bool>>();
            var command = new FakeDispatchableQuery();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            cancellationTokenSource.Cancel();

            Func<Task> act = () => dispatcher.DispatchAsync(command, cancellationTokenSource.Token);

            act.Should().Throw<OperationCanceledException>();

            ((FakeDispatchableQueryHandler)handler).Handled.Should().Be(0);
        }
        [Fact]
        public void WhenCancellationTokenRemainsNonCancelledThenShouldNotThrowOperationCancelledException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddQueries()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            var handler = serviceProvider.GetRequiredService<IQueryHandler<FakeDispatchableQuery, bool>>();
            var command = new FakeDispatchableQuery();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Func<Task> act = () => dispatcher.DispatchAsync(command, cancellationTokenSource.Token);

            act.Should().NotThrow();

            ((FakeDispatchableQueryHandler)handler).Handled.Should().Be(1);
        }
    }
}
