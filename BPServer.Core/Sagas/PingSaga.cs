using BPServer.Core.MessageBus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Sagas
{
    public class PingSaga : Saga
    {
        public PingSaga(string transportName, ICommand command, TimeSpan timeout, int maxRepeats, bool hasCommandResponse) : base(transportName, command, timeout, maxRepeats, hasCommandResponse)
        {
        }
    }
}
