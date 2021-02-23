using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands.Pipeline
{
    public class CommandOptionsBuilder
    {
        private readonly Dictionary<Type, object> _builders = new();
        private readonly IServiceCollection _services;

        public CommandOptionsBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public CommandPipelineBuilder<TCommand> For<TCommand>()
            where TCommand : ICommand
        {
            // TODO(Dan): Consider returning the same builder? Not sure if it would be better to replace on 2nd invocation of the "For<TCommand>" vs
            //            returning the previous instance and potentially chaining the additional "Use" calls?
            
            // TODO(Dan): Each builder is a "CommandPipelineBuilder<TCommand>", so we can't store them in _builders strongly typed
            //            this needs consideration. Being strongly typed makes resolving easier via generics in the default dispatcher (e.g. GetRequiredService<CommandPipeline<TCommand>>),
            //            however, it's worth considering the trade-offs
            if (_builders.TryGetValue(typeof(CommandPipelineBuilder<TCommand>), out var builder) && builder is CommandPipelineBuilder<TCommand> commandPipelineBuilder)
                return (CommandPipelineBuilder<TCommand>)builder;
            
            builder = new CommandPipelineBuilder<TCommand>(_services);

            _builders.Add(typeof(CommandPipelineBuilder<TCommand>), builder);
            
            return (CommandPipelineBuilder<TCommand>)builder;
        }

        public void Build()
        {
            // TODO(Dan): Each builder is a "CommandPipelineBuilder<TCommand>", so we can't store them in _builders strongly typed
            //            this needs consideration. Being strongly typed makes resolving easier via generics in the default dispatcher (e.g. GetRequiredService<CommandPipeline<TCommand>>),
            //            however, it's worth considering the trade-offs
            foreach (var item in _builders)
            {
                var builder = item.Value;
                var type = builder.GetType();
                var method = type.GetMethod(nameof(CommandPipelineBuilder<ICommand>.Build));

                if (method == null)
                    throw new InvalidOperationException($"Could not find '{nameof(CommandPipelineBuilder<ICommand>.Build)}' method on pipeline builder '{item.Key.FriendlyName()}'.");

                var pipeline = method.Invoke(builder, null);

                if (pipeline == null)
                    throw new InvalidOperationException($"Builder {item.Key.FriendlyName()}' returned a null command pipeline, expecting an '{method.ReturnType.FriendlyName()}'.");
                
                _services.AddSingleton(pipeline.GetType(), sp => pipeline);
            }
        }
    }
}
