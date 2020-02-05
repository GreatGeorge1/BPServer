namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMessageBus
    {
        void Publish(IMessage @message);
        void Subscribe<T, TH>(byte Route)
            where T : IMessage
            where TH : IHandler<IMessage>;
        void Unsubscribe<T,TH>(byte Route)
            where T : IMessage
            where TH : IHandler<IMessage>;
    }

    public class InMemoryMessageBus: IMessageBus, IDisposable
    {
      //  private readonly Dictionary<>

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// to transport
        /// </summary>
        /// <param name="message"></param>
        public void Publish(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>(byte Route)
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>(byte Route)
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            throw new NotImplementedException();
        }
    }
}
