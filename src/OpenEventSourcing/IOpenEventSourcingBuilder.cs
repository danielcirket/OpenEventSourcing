using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing
{
    public interface IOpenEventSourcingBuilder
    {
        IServiceCollection Services { get; }
    }
}
