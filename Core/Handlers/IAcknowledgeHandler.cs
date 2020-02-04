namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface IAcknowledgeHandler : IHandler
    {
        [Method(MessageType.ACK)]
        Task Acknowledge(AcknowledgMessage input);

        [Method(MessageType.NACK)]
        Task NegativeAcknowledge(NegativeAcknowledgeMessage input);
    }
}
