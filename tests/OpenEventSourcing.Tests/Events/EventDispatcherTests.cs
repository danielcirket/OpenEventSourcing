using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using Xunit;

namespace OpenEventSourcing.Tests.Events
{
    public class EventDispatcherTests
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcherTests()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEvents()
                    .Services
                    .AddLogging();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void WhenConstructorLoggerIsNullThenShouldThrowArgumentNullException()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            ILogger<DefaultEventDispatcher> logger = null;

            Action act = () => new DefaultEventDispatcher(logger, serviceProvider);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructorServiceProviderIsNullThenShouldThrowArgumentNullException()
        {
            IServiceProvider serviceProvider = null;
            var logger = Mock.Of<ILogger<DefaultEventDispatcher>>();

            Action act = () => new DefaultEventDispatcher(logger, serviceProvider);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("serviceProvider");
        }
        [Fact]
        public void WhenDispatchingNullEventThenShouldThrowArgumentNullException()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();

            Func<Task> act = async () => await dispatcher.DispatchAsync(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("context");
        }
        [Fact]
        public void WhenDispatchingGenericNullEventThenShouldThrowArgumentNullException()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();

            Func<Task> act = async () => await dispatcher.DispatchAsync<FakeEvent>(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("context");
        }
        [Fact]
        public void WhenNoHandlersRegisteredForEventThenShouldThrowInvalidOperationException()
        {
            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .Services
                .AddSingleton<IEventDispatcher, DefaultEventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
            var @event = new FakeEvent();
            var context = new EventContext<FakeEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);
            
            Func<Task> act = async () => await dispatcher.DispatchAsync(context);

            act.Should().Throw<InvalidOperationException>();
        }
        [Fact]
        public async Task WhenSingleHandlersRegisteredForEventThenShouldDispatchToHandler()
        {
            var @event = new FakeEvent();
            var context = new EventContext<FakeEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);

            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .AddEvents()
                .Services
                .AddSingleton<IEventDispatcher, DefaultEventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var handler = (FakeEventHandler)serviceProvider.GetRequiredService<IEventHandler<FakeEvent>>();
            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            await dispatcher.DispatchAsync(context);

            handler.Calls.Should().Be(1);
        }
        [Fact]
        public void WhenCancellationRequestedThenShouldThrowOperationCancelledException()
        {
            var @event = new FakeEvent();
            var context = new EventContext<FakeEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);

            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .AddEvents()
                .Services
                .AddSingleton<IEventDispatcher, DefaultEventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var handler = (FakeEventHandler)serviceProvider.GetRequiredService<IEventHandler<FakeEvent>>();
            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            cancellationTokenSource.Cancel();

            Func<Task> act = async () => await dispatcher.DispatchAsync(context, cancellationToken);

            act.Should().Throw<OperationCanceledException>();

            handler.Calls.Should().Be(0);
        }
        [Fact]
        public void WhenCancellationTokenRemainsNonCancelledThenShouldNotThrowOperationCancelledException()
        {
            var @event = new FakeEvent();
            var context = new EventContext<FakeEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);

            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .AddEvents()
                .Services
                .AddSingleton<IEventDispatcher, DefaultEventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var handler = (FakeEventHandler)serviceProvider.GetRequiredService<IEventHandler<FakeEvent>>();
            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Func<Task> act = async () => await dispatcher.DispatchAsync(context, cancellationToken);

            act.Should().NotThrow();

            handler.Calls.Should().Be(1);
        }
    }
}
