using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Internal;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Storage.Internal;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkCoreSqlServerJson(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddEntityFrameworkSqlServer();

            new EntityFrameworkRelationalServicesBuilder(services)
                .TryAdd<ISingletonOptions, SqlServerJsonOptions>(p => p.GetService<SqlServerJsonOptions>())
                .TryAddProviderSpecificServices(
                    x => x
                        .TryAddSingleton<SqlServerJsonOptions, SqlServerJsonOptions>()
                        .TryAddSingletonEnumerable<ISqlExpressionFactory, JsonSqlExpressionFactory>()
                        .TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, SqlServerJsonTypeMappingSourcePlugin>()
                        .TryAddSingletonEnumerable<IMemberTranslatorPlugin, SqlServerJsonMemberTranslatorPlugin>());

            services.AddSingleton<IQuerySqlGeneratorFactory, JsonQuerySqlGeneratorFactory>();
            services.AddSingleton<IRelationalSqlTranslatingExpressionVisitorFactory, JsonSqlTranslatingExpressionVisitorFactory>();

            return services;
        }
    }
}
