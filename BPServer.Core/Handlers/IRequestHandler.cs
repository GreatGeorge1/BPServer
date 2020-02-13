namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;

    public interface IRequestHandler<TCommand> : IHandler<RequestMessage,TCommand>
        where TCommand : ICommand
    {
    }
}
