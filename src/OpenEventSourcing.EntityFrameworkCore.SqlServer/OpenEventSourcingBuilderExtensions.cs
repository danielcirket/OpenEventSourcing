using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
#if NETCOREAPP3_0 || NETCOREAPP3_1
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions;
#endif

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer
{
    public static class OpenEventSourcingBuilderExtensions
    {
        /// <summary>
        /// Configures EntityFrameworkCore SqlServer with the supplied <see cref="SqlServerDbContextOptionsBuilder">.
        /// Connection strings are taken from the IConfiguration, specifically `IConfiguration.GetConnectionString("Store")` and `IConfiguration.GetConnectionString("Projection")` respectively.
        /// To have more control over connection string configuration see <see cref="AddEntityFrameworkCoreSqlServer(IOpenEventSourcingBuilder, Action{SqlServerOptions})"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Configures EntityFrameworkCore SqlServer with the supplied <see cref="SqlServerOptions">.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IOpenEventSourcingBuilder AddEntityFrameworkCoreSqlServer(this IOpenEventSourcingBuilder builder, Action<SqlServerOptions> optionsAction = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddEntityFrameworkCore();

            builder.Services.Configure(optionsAction);

            builder.Services.AddDbContext<OpenEventSourcingDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<SqlServerOptions>();
                options.UseSqlServer(opts.StoreConnectionString, opts.SqlServerOptionsBuilder);
#if NETCOREAPP3_0 || NETCOREAPP3_1
                options.EnableJsonSupport(b =>
                {
                    b.UseStrict();
                    b.UseCamelCase();
                });
#endif
            });

            builder.Services.AddDbContext<OpenEventSourcingProjectionDbContext>((sp, options) =>
            {
                var opts = sp.GetRequiredService<SqlServerOptions>();
                options.UseSqlServer(opts.ProjectionConnectionString, opts.SqlServerOptionsBuilder);
#if NETCOREAPP3_0 || NETCOREAPP3_1
                options.EnableJsonSupport(b =>
                {
                    b.UseStrict();
                    b.UseCamelCase();
                });
#endif
            });            

            return builder;
        }                 
    }
}
