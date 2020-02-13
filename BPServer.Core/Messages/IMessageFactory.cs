namespace BPServer.Core.Messages
{
    public interface IMessageFactory
    {
        bool CreateMessage(byte[] input, out IMessage message);
    }
}