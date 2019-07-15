using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreCommandStore : ICommandStore
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ICommandSerializer _commandSerializer;
        private readonly ILogger<EntityFrameworkCoreCommandStore> _logger;

        public EntityFrameworkCoreCommandStore(IDbContextFactory dbContextFactory,
                                               ICommandSerializer commandSerializer,
                                               ILogger<EntityFrameworkCoreCommandStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (commandSerializer == null)
                throw new ArgumentNullException(nameof(commandSerializer));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _commandSerializer = commandSerializer;
            _logger = logger;
        }

        public async Task SaveAsync(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var type = command.GetType();
            var data = _commandSerializer.Serialize(command);

            using (var context = _dbContextFactory.Create())
            {
                await context.Commands.AddAsync(new Entities.Command
                {
                    Name = type.Name,
                    Type = type.FullName,
                    AggregateId = command.AggregateId,
                    CorrelationId = command.CorrelationId,
                    Data = data,
                    Id = command.Id,
                    UserId = command.UserId,
                    Timestamp = command.Timestamp,
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
