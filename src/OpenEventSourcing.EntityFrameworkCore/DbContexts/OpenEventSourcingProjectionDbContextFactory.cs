using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    internal sealed class OpenEventSourcingProjectionDbContextFactory : IProjectionDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenEventSourcingProjectionDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public OpenEventSourcingProjectionDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<OpenEventSourcingProjectionDbContext>>();

            return new OpenEventSourcingProjectionDbContext(options);
        }
    }
}
