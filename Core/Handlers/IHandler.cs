namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System;
    using System.Threading.Tasks;

    public interface IHandler<in TMessage, in TCommand> : IDisposable
        where TMessage : IMessage
        where TCommand : ICommand
    {
        ICommand Command { get; }
        string SerialPort { get; }

        bool IsCompleted { get; }
        bool IsWaiting { get; }

        event EventHandler<IHandler<IMessage,ICommand>> Completed;
        event EventHandler<IHandler<IMessage, ICommand>> Waiting;
        Task Handle(TMessage input);
    }

}
