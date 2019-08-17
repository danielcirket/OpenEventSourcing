using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Stores;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Stores.QueryStore
{
    public class QueryStoreTests
    {
        [Fact]
        public void WhenConstructedWithNullDbContextFactoryShouldThrowArgumentNullException()
        {
            IDbContextFactory dbContextFactory = null;
            var querySerializer = new Mock<IQuerySerializer>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreQueryStore>>().Object;

            Action act = () => new EntityFrameworkCoreQueryStore(dbContextFactory: dbContextFactory, querySerializer: querySerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("dbContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullQuerySerializerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            IQuerySerializer querySerializer = null;
            var logger = new Mock<ILogger<EntityFrameworkCoreQueryStore>>().Object;

            Action act = () => new EntityFrameworkCoreQueryStore(dbContextFactory: dbContextFactory, querySerializer: querySerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("querySerializer");
        }
        [Fact]
        public void WhenConstructedWithNullLoggerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var querySerializer = new Mock<IQuerySerializer>().Object;
            ILogger<EntityFrameworkCoreQueryStore> logger = null;

            Action act = () => new EntityFrameworkCoreQueryStore(dbContextFactory: dbContextFactory, querySerializer: querySerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenSavingNullQueryThenShoudThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var querySerializer = new Mock<IQuerySerializer>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreQueryStore>>().Object;
            var store = new EntityFrameworkCoreQueryStore(dbContextFactory: dbContextFactory, querySerializer: querySerializer, logger: logger);
            FakeQuery query = null;

            Func<Task> act = async () => await store.SaveAsync(query);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("query");
        }

        [Fact]
        public async Task WhenSavingQueryThenShouldSaveToUnderlyingStorage()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers()
                    .Services
                    .AddLogging(configure => configure.AddConsole());

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = serviceProvider.GetRequiredService<IQueryStore>();
            var serializer = serviceProvider.GetRequiredService<IQuerySerializer>();
            var query = new FakeQuery();

            await store.SaveAsync(query);

            dbContext.Queries.Count(q => q.Id == query.Id).Should().Be(1);

            var result = await dbContext.Queries.FirstAsync(q => q.Id == query.Id);

            result.Id.Should().Be(query.Id);
            result.Timestamp.Should().Be(query.Timestamp);
            result.CorrelationId.Should().Be(query.CorrelationId);
            result.UserId.Should().Be(query.UserId);
            result.Data.Should().Be(serializer.Serialize(query));
        }
    }
}
