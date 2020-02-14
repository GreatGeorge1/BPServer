using BPServer.Core.MessageBus.Messages;

namespace BPServer.Core.MessageBus.Handlers
{
    public interface IRequestHandler<TCommand> : IHandler<RequestMessage, TCommand>
        where TCommand : ICommand
    {
    }
}
