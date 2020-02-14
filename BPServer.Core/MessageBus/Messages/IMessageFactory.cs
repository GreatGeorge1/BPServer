namespace BPServer.Core.MessageBus.Messages
{
    public interface IMessageFactory
    {
        bool CreateMessage(byte[] input, out IMessage message);
    }
}