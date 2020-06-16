using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Commands;
using Xunit;

namespace OpenEventSourcing.Tests.Commands.CommandDispatcher
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultCommandDispatcher> logger = null;
            IServiceProvider serviceProvider = null;
            ICommandStore commandStore = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, commandStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(logger));
        }
        [Fact]
        public void WhenConstructedWithNullServiceProviderThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultCommandDispatcher> logger = Mock.Of<ILogger<DefaultCommandDispatcher>>();
            IServiceProvider serviceProvider = null;
            ICommandStore commandStore = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, commandStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(serviceProvider));
        }
        [Fact]
        public void WhenConstructedWithNullCommandStoreThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultCommandDispatcher> logger = Mock.Of<ILogger<DefaultCommandDispatcher>>();
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            ICommandStore commandStore = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, commandStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(commandStore));
        }
    }
}
