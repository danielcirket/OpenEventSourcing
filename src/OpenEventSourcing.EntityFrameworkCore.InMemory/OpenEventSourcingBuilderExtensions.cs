using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.InMemory
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddEntityFrameworkCoreInMemory(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("Store");
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("Projection");
            });

            return builder;
        }
    }
}
