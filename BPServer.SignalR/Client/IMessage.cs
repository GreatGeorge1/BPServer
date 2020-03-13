using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.SignalR
{
    public interface IMessage
    {
        string MessageId { get; }
        string CorrelationId { get; }
        string RequestId { get; }
        DateTime SentTime { get; }
        string SourceAddress { get; }
        string DestinationAddress { get; }
        string ResponseAddress { get; }
        string FaultAddress { get; }
        string MessageType { get; }
        string Host { get; }
    }
}
