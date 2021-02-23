using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.Commands.Pipeline
{
    public class CommandPipelineBuilder<TCommand> where TCommand : ICommand
    {
        private readonly List<Func<CommandHandlerDelegate, CommandHandlerDelegate>> _components = new();
        private readonly IServiceCollection _services;

        public CommandPipelineBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public CommandPipelineBuilder<TCommand> Use<TCommandHandler>() where TCommandHandler : class, ICommandHandler<TCommand>
        {
            _services.AddScoped<TCommandHandler>();
            _services.AddScoped<ICommandHandler<TCommand>>(sp => sp.GetRequiredService<TCommandHandler>());

            return Use(next =>
            {
                return async (context, command, cancellationToken) =>
                {
                    var factory = context.ApplicationServices.GetService<ICommandHandlerFactory>();

                    if (factory == null)
                        throw new InvalidOperationException($"No command handler factory found for type '{nameof(ICommandHandlerFactory)}'.");

                    var handler = factory.Create<TCommandHandler>();
                    
                    if (handler == null)
                        throw new InvalidOperationException($"Unable to create command handler '{typeof(TCommandHandler).Name}' using factory '{factory.GetType().Name}'.");

                    Func<Task> simpleNext = () => next(context, command, cancellationToken);
                    
                    await handler.ExecuteAsync(simpleNext, (TCommand)command, cancellationToken);

                    //await next(context, command, cancellationToken);
                };
            });
        }
        public CommandPipelineBuilder<TCommand> Use(Func<CommandContext, Func<Task>, TCommand, CancellationToken, Task> middleware)
        {
            return Use(next =>
            {
                return (context, command, cancellationToken) =>
                {
                    Func<Task> simpleNext = () => next(context, command, cancellationToken);

                    return middleware(context, simpleNext, (TCommand)command, cancellationToken);
                };
            });
        }
        public CommandPipelineBuilder<TCommand> Use(Func<CommandHandlerDelegate, CommandHandlerDelegate> middleware)
        {
            _components.Add(middleware);

            return this;
        }

        public CommandPipeline<TCommand> Build()
        {
            CommandHandlerDelegate pipe = (context, command, cancellationToken) =>
            {
                // TODO(Dan): Do we need to detect if we're the only handler or if the command wasn't actually handled?
                //            Previous behaviour was the equivalent to having no pipeline for the command, this is different in that
                //            there is a pipeline, but its got no calls to .Use(...) so this 'pipe' is the only handler.
                //            
                //            In all likelihood, this is probably OK.
                //
                //            It's worth considering if we have some kind of "option" that you can specify when building to validate that
                //            at least 1 call to .Use(...) has been performed. Much like how in the MS DI package there is the "validateOnBuild" option.

                return Task.CompletedTask;
            };

            for (var c = _components.Count - 1; c >= 0; c--)
                pipe = _components[c](pipe);

            return new CommandPipeline<TCommand>(pipe);
        }
    }
}
