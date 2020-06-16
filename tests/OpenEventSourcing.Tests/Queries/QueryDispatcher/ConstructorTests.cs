using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Queries;
using Xunit;

namespace OpenEventSourcing.Tests.Queries.QueryDispatcher
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultQueryDispatcher> logger = null;
            IServiceProvider serviceProvider = null;
            IQueryStore queryStore = null;

            Action act = () => new DefaultQueryDispatcher(logger, serviceProvider, queryStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(logger));
        }
        [Fact]
        public void WhenConstructedWithNullServiceProviderThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultQueryDispatcher> logger = Mock.Of< ILogger<DefaultQueryDispatcher>>();
            IServiceProvider serviceProvider = null;
            IQueryStore queryStore = null;

            Action act = () => new DefaultQueryDispatcher(logger, serviceProvider, queryStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(serviceProvider));
        }
        [Fact]
        public void WhenConstructedWithNullQueryStoreThenShouldThrowArgumentNullException()
        {
            ILogger<DefaultQueryDispatcher> logger = Mock.Of<ILogger<DefaultQueryDispatcher>>();
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            IQueryStore queryStore = null;

            Action act = () => new DefaultQueryDispatcher(logger, serviceProvider, queryStore);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(queryStore));
        }
    }
}
