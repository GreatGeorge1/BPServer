using System;
using System.Threading.Tasks;
using BPServer.Core.MessageBus;
using BPServer.Core.Sagas;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Handlers;
using Serilog;
using BPServer.Core.MessageBus.Command;

namespace ConsoleApp1
{
    public class PingAcknowledgeHandler : IAcknowledgeHandler<PingCommand>
    {
        private readonly ISagasManager _sagasManager;
        private readonly ICommand _pingCommand;
        private readonly IMessageBus _messageBus;
        private readonly ILogger log;

        public PingAcknowledgeHandler(ISagasManager sagasManager, PingCommand pingCommand, IMessageBus messageBus, ILogger log)
        {
            _sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _pingCommand = pingCommand ?? throw new ArgumentNullException(nameof(pingCommand));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        private void DoHandle(IMessage input, IAddress address)
        {
            if (_sagasManager.TryGet(address.TransportName, _pingCommand, out ISaga saga))
            {
                Console.WriteLine("ping ok");
                saga.SetCompleted();
            }
            else
            {
                Console.WriteLine("ping !ok");
            }

        }

        public async Task Handle(AcknowledgeMessage input, IAddress address)
        {
            DoHandle(input, address);
        }

        public async Task Handle(NegativeAcknowledgeMessage input, IAddress address)
        {
            DoHandle(input, address);
        }
    }
}
