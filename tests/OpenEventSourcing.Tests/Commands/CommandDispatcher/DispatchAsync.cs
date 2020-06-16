using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Tests.Fakes;
using Xunit;

namespace OpenEventSourcing.Tests.Commands.CommandDispatcher
{
    public class DispatchAsyncTests
    {
        [Fact]
        public void WhenCommandIsNullThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddCommands()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            FakeCommand command = null;

            Func<Task> act = () => dispatcher.DispatchAsync(command);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(command));
        }
        [Fact]
        public void WhenNoHandlerRegisteredForCommandThenShouldThrowInvalidOperationException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddCommands()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var command = new FakeCommand();

            Func<Task> act = () => dispatcher.DispatchAsync(command);

            act.Should().Throw<InvalidOperationException>()
                .And.Message.Should().Be($"No command handler for type '{typeof(FakeCommand).FriendlyName()}' has been registered.");
        }
        [Fact]
        public void WhenHandlerRegisteredForCommandThenShouldDispatchCommandToHandler()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddCommands()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var handler = serviceProvider.GetRequiredService<ICommandHandler<FakeDispatchableCommand>>();
            var command = new FakeDispatchableCommand();

            Func<Task> act = () => dispatcher.DispatchAsync(command);

            act.Should().NotThrow();

            ((FakeDispatchableCommandHandler)handler).Handled.Should().Be(1);
        }
        [Fact]
        public void WhenCancellationRequestedThenShouldThrowOperationCancelledException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddCommands()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var handler = serviceProvider.GetRequiredService<ICommandHandler<FakeDispatchableCommand>>();
            var command = new FakeDispatchableCommand();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            cancellationTokenSource.Cancel();

            Func<Task> act = () => dispatcher.DispatchAsync(command, cancellationTokenSource.Token);

            act.Should().Throw<OperationCanceledException>();

            ((FakeDispatchableCommandHandler)handler).Handled.Should().Be(0);
        }
        [Fact]
        public void WhenCancellationTokenRemainsNonCancelledThenShouldNotThrowOperationCancelledException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddCommands()
                    .Services
                    .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var handler = serviceProvider.GetRequiredService<ICommandHandler<FakeDispatchableCommand>>();
            var command = new FakeDispatchableCommand();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Func<Task> act = () => dispatcher.DispatchAsync(command, cancellationTokenSource.Token);

            act.Should().NotThrow();

            ((FakeDispatchableCommandHandler)handler).Handled.Should().Be(1);
        }
    }
}
