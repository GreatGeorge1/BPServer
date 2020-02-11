namespace BPServer.Core.MessageBus
{
    using System;

    public partial class InMemoryMessageBus
    {
        public class ExceptionReceivedEventArgs
        {
            public ExceptionReceivedEventArgs(Exception exception, Type exceptionReceivedContext)
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
                ExceptionReceivedContext = exceptionReceivedContext ?? throw new ArgumentNullException(nameof(exceptionReceivedContext));
            }

            public Exception Exception { get; protected set; }
            public Type ExceptionReceivedContext { get; protected set; }
        }
    }
}
