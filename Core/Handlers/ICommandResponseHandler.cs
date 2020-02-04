namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface ICommandResponseHandler<TCommand> : IAcknowledgeHandler
       where TCommand : ICommand
    {
        [Method(MessageType.CommandResponse)]
        Task CommandResponse(CommandResponseMessage input);
    }
}
