using System;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Domain;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.Projections;
using OpenEventSourcing.EntityFrameworkCore.Stores;
using OpenEventSourcing.Events;
using OpenEventSourcing.Projections;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.EntityFrameworkCore.Extensions
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddEntityFrameworkCore(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ICommandStore, EntityFrameworkCoreCommandStore>();
            builder.Services.AddScoped<IEventStore, EntityFrameworkCoreEventStore>();
            builder.Services.AddScoped<IQueryStore, EntityFrameworkCoreQueryStore>();
            builder.Services.AddScoped<IAggregateRepository, AggregateRepository>();
            builder.Services.AddScoped<IEventModelFactory, DefaultEventModelFactory>();
            builder.Services.AddScoped<IDbContextFactory, OpenEventSourcingDbContextFactory>();
            builder.Services.AddScoped<IProjectionDbContextFactory, OpenEventSourcingProjectionDbContextFactory>();
            builder.Services.AddScoped<IEventModelFactory, DefaultEventModelFactory>();
            builder.Services.AddScoped(typeof(IProjectionWriter<>), typeof(EntityFrameworkCoreProjectionWriter<>));

            return builder;
        }
    }
}
