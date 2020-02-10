namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;

    public interface IMessageBus
    {
        void Publish(IMessage @message, string serialPort);
        void Subscribe<T, TH>(string serialPort)
            where T : IMessage
            where TH : IHandler<IMessage,ICommand>;
        void Unsubscribe<T,TH>(string serialPort)
            where T : IMessage
            where TH : IHandler<IMessage,ICommand>;
    }
}
