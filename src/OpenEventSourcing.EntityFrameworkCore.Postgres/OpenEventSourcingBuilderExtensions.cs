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
        /// <summary>
        /// Configures EntityFrameworkCore Postgres with the supplied <see cref="NpgsqlDbContextOptionsBuilder">.
        /// Connection strings are taken from the IConfiguration, specifically `IConfiguration.GetConnectionString("Store")` and `IConfiguration.GetConnectionString("Projection")` respectively.
        /// To have more control over connection string configuration see <see cref="AddEntityFrameworkCorePostgres(IOpenEventSourcingBuilder, Action{NpgsqlOptions})"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Configures EntityFrameworkCore Postgres with the supplied <see cref="NpgsqlOptions">.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IOpenEventSourcingBuilder AddEntityFrameworkCorePostgres(this IOpenEventSourcingBuilder builder, Action<NpgsqlOptions> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<NpgsqlOptions>();
                options.UseNpgsql(opts.StoreConnectionString, opts.NpgsqlOptionsBuilder);
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<NpgsqlOptions>();
                options.UseNpgsql(opts.ProjectionConnectionString, opts.NpgsqlOptionsBuilder);
            });

            return builder;
        }
    }
}
