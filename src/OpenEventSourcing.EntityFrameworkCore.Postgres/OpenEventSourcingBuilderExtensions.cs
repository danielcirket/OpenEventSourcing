using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Postgres
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddEntityFrameworkCorePostgres(this IOpenEventSourcingBuilder builder, Action<NpgsqlDbContextOptionsBuilder> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Store");

                options.UseNpgsql(connectionString, optionsAction);
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Projection");

                options.UseNpgsql(connectionString, optionsAction);
            });

            return builder;
        }
    }
}
