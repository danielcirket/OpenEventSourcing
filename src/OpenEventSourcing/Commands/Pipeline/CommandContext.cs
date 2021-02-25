using System;

namespace OpenEventSourcing.Commands.Pipeline
{
    public class CommandContext
    {
        public IServiceProvider ApplicationServices { get; }

        public CommandContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            
            ApplicationServices = serviceProvider;
        }
    }
}
