using System.Threading.Tasks;

namespace OpenEventSourcing.Commands
{
    public interface ICommandStore
    {
        Task SaveAsync(ICommand command);
    }
}
