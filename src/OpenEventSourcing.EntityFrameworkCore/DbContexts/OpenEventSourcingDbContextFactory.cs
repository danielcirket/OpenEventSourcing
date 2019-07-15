using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    internal sealed class OpenEventSourcingDbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenEventSourcingDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public OpenEventSourcingDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<OpenEventSourcingDbContext>>();

            return new OpenEventSourcingDbContext(options);
        }
    }
}
