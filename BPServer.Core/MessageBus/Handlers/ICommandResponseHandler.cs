using BPServer.Core.MessageBus.Messages;

namespace BPServer.Core.MessageBus.Handlers
{
    public interface ICommandResponseHandler<TCommand> : IHandler<CommandResponseMessage, TCommand>
       where TCommand : ICommand
    {
    }
}
