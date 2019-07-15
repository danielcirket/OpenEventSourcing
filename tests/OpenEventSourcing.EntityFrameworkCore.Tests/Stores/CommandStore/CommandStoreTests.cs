using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Commands;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Stores;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Stores.CommandStore
{
    public class CommandStoreTests
    {
        [Fact]
        public void WhenConstructedWithNullDbContextFactoryShouldThrowArgumentNullException()
        {
            IDbContextFactory dbContextFactory = null;
            var commandSerializer = new Mock<ICommandSerializer>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreCommandStore>>().Object;

            Action act = () => new EntityFrameworkCoreCommandStore(dbContextFactory: dbContextFactory, commandSerializer: commandSerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("dbContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullQuerySerializerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            ICommandSerializer commandSerializer = null;
            var logger = new Mock<ILogger<EntityFrameworkCoreCommandStore>>().Object;

            Action act = () => new EntityFrameworkCoreCommandStore(dbContextFactory: dbContextFactory, commandSerializer: commandSerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("commandSerializer");
        }
        [Fact]
        public void WhenConstructedWithNullLoggerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var commandSerializer = new Mock<ICommandSerializer>().Object;
            ILogger<EntityFrameworkCoreCommandStore> logger = null;

            Action act = () => new EntityFrameworkCoreCommandStore(dbContextFactory: dbContextFactory, commandSerializer: commandSerializer, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenSavingNullQueryThenShoudThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var commandSerializer = new Mock<ICommandSerializer>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreCommandStore>>().Object;
            var store = new EntityFrameworkCoreCommandStore(dbContextFactory: dbContextFactory, commandSerializer: commandSerializer, logger: logger);
            FakeCommand command = null;

            Func<Task> act = async () => await store.SaveAsync(command);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("command");
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
            var store = serviceProvider.GetRequiredService<ICommandStore>();
            var serializer = serviceProvider.GetRequiredService<IQuerySerializer>();
            var command = new FakeCommand();

            await store.SaveAsync(command);

            dbContext.Commands.Count(c => c.Id == command.Id).Should().Be(1);

            var result = await dbContext.Commands.FirstAsync(c => c.Id == command.Id);

            result.Id.Should().Be(command.Id);
            result.Timestamp.Should().Be(command.Timestamp);
            result.CorrelationId.Should().Be(command.CorrelationId);
            result.UserId.Should().Be(command.UserId);
            result.Name.Should().Be(command.GetType().Name);
            result.Type.Should().Be(command.GetType().FullName);
            result.Data.Should().Be(serializer.Serialize(command));
        }
    }
}
