using BPServer.Core.Messages;

namespace BPServer.Core.Handlers
{
    public interface ICommandHandler<TCommand> : IAcknowledgeHandler, IHandler<CommandMessage>
        where TCommand : ICommand
    {

    }
}
