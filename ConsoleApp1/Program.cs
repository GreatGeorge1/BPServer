using BPServer.Core.Attributes;
using BPServer.Core.Handlers;
using BPServer.Core.Messages;
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

namespace ConsoleApp1
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<HubModule>();

            builder.RegisterType<TestTransport>().As<ITransport>().SingleInstance();
            builder.RegisterType<CardNotificationHandler>();
            builder.RegisterType<CardCommand>();
            var container = builder.Build();



            container.BeginLifetimeScope();
            var transportManager = container.Resolve<ITransportManager>();
            var transport = container.Resolve<ITransport>();
            transportManager.AddTransport(transport);
            var messageBus = container.Resolve<IMessageBus>();
            messageBus.Subscribe<CardNotificationHandler>("TestTransport");
            Console.ReadLine();


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

    [CommandByte(0xC7)]
    public class CardCommand : ICommand
    {
        public byte Command => 0xC7;
    }

    public class SomeSaga : Saga
    {
        public SomeSaga(string transportName, ICommand command, TimeSpan timeout, int maxRepeats, bool hasCommandResponse) : base(transportName, command, timeout, maxRepeats, hasCommandResponse)
        {
        }
    }

    public class CardNotificationHandler : INotificationHandler<CardCommand>
    {
        private readonly ISagasManager _sagasManager;
        private readonly CardCommand _cardCommand;
        private readonly IMessageBus _messageBus;
        public CardNotificationHandler(ISagasManager sagasManager, CardCommand command, IMessageBus messageBus)
        {
            _sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _cardCommand = command ?? throw new ArgumentNullException(nameof(command));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }
        public async Task Handle(NotificationMessage input, IAddress address)
        {
            Console.WriteLine("Handling NotificationMessage for CardCommand");
            if(_sagasManager.TryGet(address.TransportName,_cardCommand, out ISaga saga))
            {
                Console.WriteLine("saga resovled");
                saga.SetCompleted();
            }
            else
            {
                _sagasManager.AddSaga(new SomeSaga(address.TransportName, _cardCommand, TimeSpan.Zero, 3, false));
                Console.WriteLine("saga created");
                await _messageBus.Publish(input, address.TransportName).ConfigureAwait(false);
            }

        }
    }
}
