using System;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Extensions
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddCommands(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.AssignableTo<ICommandDispatcher>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            return builder;
        }
        public static IOpenEventSourcingBuilder AddEvents(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.AssignableTo<IEventDispatcher>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            return builder;
        }
        public static IOpenEventSourcingBuilder AddQueries(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.AssignableTo<IQueryDispatcher>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            return builder;
        }
    }
}
