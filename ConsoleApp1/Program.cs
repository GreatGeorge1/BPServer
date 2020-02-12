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

namespace ConsoleApp1
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TransportManager()).As<ITransportManager>();
            builder.RegisterInstance(new InMemoryMessageBusSubscriptionsManager()).As<IMessageBusSubscriptionManager>();
            builder.RegisterType<MessageBus>().As<IMessageBus>();
            builder.RegisterInstance(new TestTransport()).As<ITransport>();
            builder.RegisterInstance(new SagasManager()).As<ISagasManager>();
            builder.RegisterType<CardNotificationHandler>();
            builder.RegisterType<CardCommand>();
            var container = builder.Build();

            container.BeginLifetimeScope();
            var transportManager= container.Resolve<ITransportManager>();
            var transport = container.Resolve<ITransport>();
            transportManager.AddTransport(transport);
            var messageBus=container.Resolve<IMessageBus>();
            messageBus.Subscribe<CardNotificationHandler>("TestTransport");
            Console.ReadLine();
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
        public CardNotificationHandler(ISagasManager sagasManager, CardCommand command)
        {
            _sagasManager = sagasManager ?? throw new ArgumentNullException(nameof(sagasManager));
            _cardCommand = command ?? throw new ArgumentNullException(nameof(command));
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
            }

        }
    }
}
