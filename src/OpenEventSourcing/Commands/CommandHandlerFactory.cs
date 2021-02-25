using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.Commands
{
    internal sealed class  CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public TCommandHandler Create<TCommandHandler>() => _serviceProvider.GetService<TCommandHandler>();
    }
}
