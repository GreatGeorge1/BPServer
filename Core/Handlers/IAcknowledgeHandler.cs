namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface IAcknowledgeHandler : IHandler<AcknowledgeMessage>, IHandler<NegativeAcknowledgeMessage>
    {
      
    }
}
