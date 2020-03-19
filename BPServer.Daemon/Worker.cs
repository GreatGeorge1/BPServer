using BPServer.Autofac.Serial;
using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Command;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.Sagas;
using BPServer.Core.Transports;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BPServer.Daemon
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IMessageFactory _messageFactory;
        private readonly ISagasManager _sagasManager;
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
      //  private readonly IBusControl _masstransitBc;
       // private readonly IPublishEndpoint _publishEndpoint;
      //  private readonly ISendEndpointProvider _sendEndpoint;

        public Worker(ILogger<Worker> logger,
            IMessageFactory messageFactory,
            ISagasManager sagasManager,
            IMessageBus messageBus,
            IServiceProvider serviceProvider
           // IBusControl busControl,
           // IPublishEndpoint publishEndpoint,
           // ISendEndpointProvider sendEndpoint
           )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
           // _masstransitBc = busControl ?? throw new ArgumentNullException(nameof(busControl));
           // _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
           // _sendEndpoint = sendEndpoint ?? throw new ArgumentNullException(nameof(sendEndpoint));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var transport = new TestTransport(
            //    (ILogger<TestTransport>)_serviceProvider.GetService(typeof(ILogger<TestTransport>))
            //    );
            var transport2 = new SerialPortTransport(
              (IMessageFactory)_serviceProvider.GetService(typeof(IMessageFactory)),
              "/dev/ttyS1",
              isRS485: true,
              (ILogger<SerialPortTransport>)_serviceProvider.GetService(typeof(ILogger<SerialPortTransport>))
              );
            //_messageBus.Subscribe<CardNotificationHandler>(transport.Name);
            //_messageBus.Subscribe<BleNotificationHandler>(transport.Name);
            //_messageBus.Subscribe<FingerNotificationHandler>(transport.Name);
            //_messageBus.Subscribe<PingAcknowledgeHandler>(transport.Name);
            _messageBus.Subscribe<CardNotificationHandler>(transport2.Name);
            _messageBus.Subscribe<BleNotificationHandler>(transport2.Name);
            _messageBus.Subscribe<FingerNotificationHandler>(transport2.Name);
            //_messageBus.Subscribe<PingAcknowledgeHandler>(transport2.Name);
            //_messageBus.TransportAdded += OnTransportAdded;
            _messageBus.AddTransport(transport2);


            // await _masstransitBc.StartAsync();
            //for (int i = 0; i < 10; i++)
            //{
            //    await _publishEndpoint.Publish<TestEvent>(new
            //    {
            //        Text = "test message"
            //    });
            //    var endpoint = await _sendEndpoint.GetSendEndpoint(new Uri(String.Concat("queue:", Routes.HubPrefix, Program.HubId, "test_consumer")));
            //    await endpoint.Send<TestEvent>(new
            //    {
            //        Text = "test message"
            //    });
            //}
        }

        private async void OnTransportAdded(object sender, TransportAddedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            _logger.LogInformation("OnTransportAdded handler");
            for (int i = 0; i < 100; i++)
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(1000));
                _sagasManager.AddSaga(new PingSaga(e.TransportName, new PingCommand(), TimeSpan.Zero, 3, false));
                await _messageBus.Publish(new CommandMessage(0xD4, new byte[] { 0x00, 0x01, 0x0A }), e.TransportName);
                //  Console.WriteLine("OnTransportAdded message pushed");
            }
            if (_sagasManager.TryGetSagas("/dev/ttyS1", out ICollection<ISaga> sagas))
            {
                _logger.LogInformation($"not complete:'{sagas.Count}' out of '100'");
            }
        }
    }
}
