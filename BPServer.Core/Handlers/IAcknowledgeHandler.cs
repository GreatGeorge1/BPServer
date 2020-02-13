namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface IAcknowledgeHandler<in TCommand> : IHandler<AcknowledgeMessage,TCommand>, IHandler<NegativeAcknowledgeMessage,TCommand>
        where TCommand : ICommand
    {
      
    }
}
