using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Commands.Pipeline;
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

            builder.Services.AddScoped<ICommandStore, NoOpCommandStore>();

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

        public static IOpenEventSourcingBuilder AddCommands(this IOpenEventSourcingBuilder builder, Action<CommandOptionsBuilder> setupAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ICommandStore, NoOpCommandStore>();
            builder.Services.AddScoped<ICommandDispatcher, DefaultCommandDispatcher>();
            builder.Services.AddScoped<CommandContext>();
            builder.Services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            
            var optionsBuilder = new CommandOptionsBuilder(builder.Services);

            setupAction?.Invoke(optionsBuilder);
            
            optionsBuilder.Build();

            return builder;
        }
        public static IOpenEventSourcingBuilder AddEvents(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            builder.Services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();

                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IEventDispatcher>(), publicOnly: false)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime();
            });

            return builder;
        }
        public static IOpenEventSourcingBuilder AddQueries(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IQueryStore, NoOpQueryStore>();

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
