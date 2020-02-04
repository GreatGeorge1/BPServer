namespace BPServer.Core.Handlers
{
    public interface ICommandHandler<TCommand> : IAcknowledgeHandler
        where TCommand : ICommand
    {

    }
}
