namespace OpenEventSourcing.Commands
{
    public interface ICommandHandlerFactory
    {
        public TCommandHandler Create<TCommandHandler>();
    }
}
