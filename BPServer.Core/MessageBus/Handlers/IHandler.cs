using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Messages;
using System.Threading.Tasks;

namespace BPServer.Core.MessageBus.Handlers
{
    public interface IHandler<in TMessage, in TCommand> : IHandler
        where TMessage : IMessage
        where TCommand : ICommand
    {
        Task Handle(TMessage @input, IAddress address);
    }

    public interface IHandler
    {

    }

}
