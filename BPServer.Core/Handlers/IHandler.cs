namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System;
    using System.Threading.Tasks;

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
