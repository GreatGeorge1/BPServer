using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BPServer.Autofac.Serial;
using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Command;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.Sagas;
using BPServer.Core.Transports;
using ConsoleApp1;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BPServer.Worker
{
    public class Worker : BackgroundService
    {
        private readonly Serilog.ILogger _logger;
        private readonly ITransportManager _transportManager;
        private readonly IMessageFactory _messageFactory;
        private readonly ISagasManager _sagasManager;
        private readonly IMessageBus _messageBus;

        public Worker(Serilog.ILogger logger,
            ITransportManager transportManager,
            IMessageFactory messageFactory,
            ISagasManager sagasManager,
            IMessageBus messageBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transportManager = transportManager ?? throw new ArgumentNullException(nameof(transportManager));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _transportManager.TransportAdded += OnTransportAdded;
            _messageBus.Subscribe<CardNotificationHandler>("/dev/ttyS1");
            _messageBus.Subscribe<BleNotificationHandler>("/dev/ttyS1");
            _messageBus.Subscribe<FingerNotificationHandler>("/dev/ttyS1");
            _messageBus.Subscribe<PingAcknowledgeHandler>("/dev/ttyS1");
            var transport = new TestTransport(_logger);
            _transportManager.AddTransport(transport);
        }

        private async void OnTransportAdded(object sender, TransportAddedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            for (int i = 0; i < 100; i++)
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(1000));
                _logger.Information("OnTransportAdded handler");
                _sagasManager.AddSaga(new PingSaga(e.TransportName, new PingCommand(), TimeSpan.Zero, 3, false));
                await _messageBus.Publish(new CommandMessage(0xD4, new byte[] { 0x00, 0x01, 0x0A }), e.TransportName);
                //  Console.WriteLine("OnTransportAdded message pushed");
            }
            if (_sagasManager.TryGetSagas("/dev/ttyS1", out ICollection<ISaga> sagas))
            {
                _logger.Information($"not complete:'{sagas.Count}' out of '100'");
            }
        }
    }
}
