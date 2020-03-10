using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Sqlite
{
    public static class OpenEventSourcingBuilderExtensions
    {
        /// <summary>
        /// Configures EntityFrameworkCore Sqlite with the supplied <see cref="SqliteDbContextOptionsBuilder">.
        /// Connection strings are taken from the IConfiguration, specifically `IConfiguration.GetConnectionString("Store")` and `IConfiguration.GetConnectionString("Projection")` respectively.
        /// To have more control over connection string configuration see <see cref="AddEntityFrameworkCoreSqlite(IOpenEventSourcingBuilder, Action{SqliteOptions})"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IOpenEventSourcingBuilder AddEntityFrameworkCoreSqlite(this IOpenEventSourcingBuilder builder, Action<SqliteDbContextOptionsBuilder> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Store");

                options.UseSqlite(connectionString, optionsAction);
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Projection");

                options.UseSqlite(connectionString, optionsAction);
            });

            return builder;
        }
        /// <summary>
        /// Configures EntityFrameworkCore Sqlite with the supplied <see cref="SqliteOptions">.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IOpenEventSourcingBuilder AddEntityFrameworkCoreSqlite(this IOpenEventSourcingBuilder builder, Action<SqliteOptions> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<SqliteOptions>();
                options.UseSqlite(opts.StoreConnectionString, opts.SqliteOptionsBuilder);
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<SqliteOptions>();
                options.UseSqlite(opts.ProjectionConnectionString, opts.SqliteOptionsBuilder);
            });

            return builder;
        }
    }
}
