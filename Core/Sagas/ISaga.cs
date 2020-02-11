using BPServer.Core.Handlers;
using BPServer.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Sagas
{
    public interface ISaga {
        string SerialPort { get; }
        ICommand Command { get; }
        Guid Id { get; }

        bool IsError { get; }
        bool IsTimeout { get; }
        bool IsCompleted { get; }
        bool IsRepeatLimitReached { get; }

        TimeSpan Timeout { get; }
        int MaxRepeats { get; }
        int RepeatCount { get; }

        event EventHandler<Guid> RepeatLimitReached;
        event EventHandler<Guid> Completed;
        event EventHandler<Guid> Error;
        event EventHandler<Guid> TimeoutReached;

        DateTime CreationTime { get; }
        IMessage Ack { get; }
        void SetAck(IMessage message);
        void RepeatIncrement();

        #region client
        IMessage RequestMessage { get; }
        IMessage SentResponse { get; }
        void SetSentResponse(IMessage message);
        void SetRequestMesssage(IMessage message);
        #endregion

        #region server
        bool HasResponse { get; }
        IMessage CommandMessage { get; }
        IMessage RecievedResponse { get; }
        void SetCommandMessage(IMessage message);
        void SetResponse(IMessage recievedResponse);
        #endregion

        void SetCompleted();
        void SetError();
    }
}
