using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using System.Data.SqlClient;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddEntityFrameworkCoreSqlServer(this IOpenEventSourcingBuilder builder, Action<SqlServerDbContextOptionsBuilder> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Store");

                options.UseSqlServer(connectionString, optionsAction);
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Projection");

                options.UseSqlServer(connectionString, optionsAction);
            });

            return builder;
        }
    }
}
