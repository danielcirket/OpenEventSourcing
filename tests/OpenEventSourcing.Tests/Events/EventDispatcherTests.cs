using System;
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
            ILogger<EventDispatcher> logger = null;

            Action act = () => new EventDispatcher(logger, serviceProvider);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructorServiceProviderIsNullThenShouldThrowArgumentNullException()
        {
            IServiceProvider serviceProvider = null;
            var logger = Mock.Of<ILogger<EventDispatcher>>();

            Action act = () => new EventDispatcher(logger, serviceProvider);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("serviceProvider");
        }
        [Fact]
        public void WhenDispatchingNullEventThenShouldThrowArgumentNullException()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();

            Func<Task> act = async () => await dispatcher.DispatchAsync(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("event");
        }
        [Fact]
        public void WhenDispatchingGenericNullEventThenShouldThrowArgumentNullException()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();

            Func<Task> act = async () => await dispatcher.DispatchAsync<FakeEvent>(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("event");
        }
        [Fact]
        public void WhenNoHandlersRegisteredForEventThenShouldThrowInvalidOperationException()
        {
            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .Services
                .AddSingleton<IEventDispatcher, EventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            Func<Task> act = async () => await dispatcher.DispatchAsync(new FakeEvent());

            act.Should().Throw<InvalidOperationException>();
        }
        [Fact]
        public async Task WhenSingleHandlersRegisteredForEventThenShouldDispatchToHandler()
        {
            var @event = new FakeEvent();

            var serviceProvider = new ServiceCollection()
                .AddOpenEventSourcing()
                .AddEvents()
                .Services
                .AddSingleton<IEventDispatcher, EventDispatcher>()
                .AddLogging()
                .BuildServiceProvider();

            var handler = (FakeEventHandler)serviceProvider.GetRequiredService<IEventHandler<FakeEvent>>();
            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            await dispatcher.DispatchAsync(@event);

            handler.Calls.Should().Be(1);
        }
    }
}
