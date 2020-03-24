namespace BPServer.Core.MessageBus
{
    using BPServer.Core.MessageBus.Messages;
    using System;

    public partial class SerialMessageBus
    {
        public class ExceptionReceivedEventArgs
        {
            public ExceptionReceivedEventArgs(Exception exception, IMessage message, string exchange//, Type exceptionReceivedContext
                )
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
                Message = message ?? throw new ArgumentNullException(nameof(message));
                Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
                //ExceptionReceivedContext = exceptionReceivedContext ?? throw new ArgumentNullException(nameof(exceptionReceivedContext));
            }

            public IMessage Message { get; protected set; }
            public Exception Exception { get; protected set; }
            public string Exchange { get; protected set; }
            //public Type ExceptionReceivedContext { get; protected set; }
        }
    }
}
