using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPServer.Core.Extentions;
using Autofac;
using BPServer.Core.Transports;
using BPServer.Core.MessageBus;
using BPServer.Core.Sagas;
using BPServer.Autofac;
using BPServer.Transports;
using System.IO.Ports;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Handlers;
using BPServer.Core.MessageBus.Attributes;
using BPServer.Autofac.Serial;
using BPServer.Core.MessageBus.Command;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static IMessageBus Bus;
        static ISagasManager SagaManager;
        static IMessageFactory Factory = new MessageFactory();
        static async void OnTransportAdded(object sender,  TransportAddedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            for (int i = 0; i < 100; i++)
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(1000));
                Console.WriteLine("OnTransportAdded handler");
                SagaManager.AddSaga(new PingSaga(e.TransportName, new PingCommand(), TimeSpan.Zero, 3, false));
                await Bus.Publish(new CommandMessage(0xD4, new byte[] { 0x00, 0x01, 0x0A }), e.TransportName);
                //  Console.WriteLine("OnTransportAdded message pushed");
            }
            if (SagaManager.TryGetSagas("/dev/ttyS1", out ICollection<ISaga> sagas))
            {
                Console.WriteLine($"not complete:'{sagas.Count}' out of '100'");
            }
        }

        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<HubModule>();

            builder.RegisterType<CardNotificationHandler>();
            builder.RegisterType<CardCommand>();
            builder.RegisterType<FingerNotificationHandler>();
            builder.RegisterType<FingerCommand>();
            builder.RegisterType<BleNotificationHandler>();
            builder.RegisterType<BleCommand>();
            builder.RegisterType<PingCommand>();
            builder.RegisterType<PingAcknowledgeHandler>();

            var container = builder.Build();

            container.BeginLifetimeScope();
            var transportManager = container.Resolve<ITransportManager>();
            transportManager.TransportAdded += OnTransportAdded;
            var logger = container.Resolve<ILogger>();
            var mfactory = container.Resolve<IMessageFactory>();
            SagaManager = container.Resolve<ISagasManager>();

            var messageBus = container.Resolve<IMessageBus>();
            Bus = messageBus;

            messageBus.Subscribe<CardNotificationHandler>("/dev/ttyS1");
            messageBus.Subscribe<BleNotificationHandler>("/dev/ttyS1");
            messageBus.Subscribe<FingerNotificationHandler>("/dev/ttyS1");
            messageBus.Subscribe<PingAcknowledgeHandler>("/dev/ttyS1");
           // var transport = new SerialPortTransport(mfactory, "/dev/ttyS1", true, logger);
           // transportManager.AddTransport(transport);



            while (true)
            {
                if (Console.ReadKey().KeyChar == 'q')
                {
                    break;
                }
            }


            //var type = typeof(IMessage);
            //var types = AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(s => s.GetTypes())
            //    .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
            //foreach(var item in types)
            //{
            //    Console.WriteLine(item.Name);
            //}

            //var factory = container.Resolve<IMessageFactory>();
            //var len = Message.IntToHighLow(2);
            //var body = new byte[] { 0x01, 0x00 };
            //var mm = new List<byte> { 0x02, 0xd5, 0xc7 };
            //mm.Add(Message.CalCheckSum(body));
            //mm.AddRange(Message.IntToHighLow(body.Length));
            //mm.AddRange(body);
            //factory.CreateMessage(mm.ToArray(), out IMessage message);
            //Console.WriteLine(message.GetType().Name);
            //var mtype = message.GetType().GetAttributeValue((MessageTypeAttribute m) => m.MessageType);
            //Console.WriteLine(BitConverter.ToString(new byte[] { mtype}));
            //string[] ports;
            //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            //{
            //    ports = SerialPort.GetPortNames();
            //}else
            //{
            //    ports = PortHelpers.GetPortNames();
            //}   

            //foreach (var item in ports)
            //{
            //    Console.WriteLine(item);
            //}
        }
    }


    #region commands
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
    #endregion
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
            log.LogInformation("{@Address}; message body: '{body}'",address, BitConverter.ToString(input.Body.ToArray()));
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
