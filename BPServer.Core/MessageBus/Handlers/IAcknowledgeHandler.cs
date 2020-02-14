using BPServer.Core.MessageBus.Messages;

namespace BPServer.Core.MessageBus.Handlers
{
    public interface IAcknowledgeHandler<in TCommand> : IHandler<AcknowledgeMessage, TCommand>, IHandler<NegativeAcknowledgeMessage, TCommand>
        where TCommand : ICommand
    {

    }
}
