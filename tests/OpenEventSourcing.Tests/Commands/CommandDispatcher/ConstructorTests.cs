using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Commands.Pipeline;
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
            CommandContext context = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, context);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(logger));
        }
        [Fact]
        public void WhenConstructedWithNullServiceProviderThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultCommandDispatcher> logger = Mock.Of<ILogger<DefaultCommandDispatcher>>();
            IServiceProvider serviceProvider = null;
            CommandContext context = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, context);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(serviceProvider));
        }
        [Fact]
        public void WhenConstructedWithNullCommandStoreThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultCommandDispatcher> logger = Mock.Of<ILogger<DefaultCommandDispatcher>>();
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            CommandContext context = null;

            Action act = () => new DefaultCommandDispatcher(logger, serviceProvider, context);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(context));
        }
    }
}
