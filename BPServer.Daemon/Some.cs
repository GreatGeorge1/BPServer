using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Attributes;
using BPServer.Core.MessageBus.Handlers;
using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.Sagas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPServer.Daemon
{
    [CommandByte(0xC7)]
    public class CardCommand : ICommand
    {
        public byte Command => 0xC7;
    }
    [CommandByte(0xF7)]
    public class FingerCommand : ICommand
    {
        public byte Command => 0xF7;
    }

    [CommandByte(0xB7)]
    public class BleCommand : ICommand
    {
        public byte Command => 0xB7;
    }

    public class SomeSaga : Saga
    {
        public SomeSaga(string transportName, ICommand command, TimeSpan timeout, int maxRepeats, bool hasCommandResponse) : base(transportName, command, timeout, maxRepeats, hasCommandResponse)
        {
        }
    }

    public class CardNotificationHandler : INotificationHandler<CardCommand>
    {
        //private readonly ISagasManager _sagasManager;
        private readonly ICommand _cardCommand;
        private readonly IMessageBus _messageBus;
        private readonly ILogger log;
        public CardNotificationHandler(CardCommand command, ILogger<CardNotificationHandler> logger, IMessageBus messageBus)
        {
            //_sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _cardCommand = command ?? throw new ArgumentNullException(nameof(command));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            log = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(NotificationMessage input, IAddress address)
        {
            log.LogInformation("{@Address}; message body: '{body}'", address, BitConverter.ToString(input.Body.ToArray()));
            var message = new AcknowledgeMessage(_cardCommand.Command, new byte[] { 0x00, 0x01 });
            await _messageBus.Publish(message, address.TransportName);
        }
    }

    public class FingerNotificationHandler : INotificationHandler<FingerCommand>
    {
        //private readonly ISagasManager _sagasManager;
        private readonly ICommand _fingerCommand;
        private readonly IMessageBus _messageBus;
        private readonly ILogger log;
        public FingerNotificationHandler(FingerCommand command, ILogger<FingerNotificationHandler> logger, IMessageBus messageBus)
        {
            //_sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _fingerCommand = command ?? throw new ArgumentNullException(nameof(command));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            log = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(NotificationMessage input, IAddress address)
        {
            log.LogInformation("{@Address}; message body: '{body}'", address, BitConverter.ToString(input.Body.ToArray()));
            var message = new AcknowledgeMessage(_fingerCommand.Command, new byte[] { 0x00, 0x01 });
            await _messageBus.Publish(message, address.TransportName);
        }
    }

    public class BleNotificationHandler : INotificationHandler<BleCommand>
    {
        //private readonly ISagasManager _sagasManager;
        private readonly ICommand _bleCommand;
        private readonly IMessageBus _messageBus;
        private readonly ILogger log;
        public BleNotificationHandler(BleCommand command, ILogger<BleNotificationHandler> logger, IMessageBus messageBus)
        {
            //_sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _bleCommand = command ?? throw new ArgumentNullException(nameof(command));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            log = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(NotificationMessage input, IAddress address)
        {
            log.LogInformation("{@Address}; message body: '{body}'", address, BitConverter.ToString(input.Body.ToArray()));
            var message = new AcknowledgeMessage(_bleCommand.Command, new byte[] { 0x00, 0x01 });
            await _messageBus.Publish(message, address.TransportName);
        }
    }
}
