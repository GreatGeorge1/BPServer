using Autofac;
using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.Sagas;
using BPServer.Core.Transports;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Autofac
{
    public class HubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransportManager>().As<ITransportManager>().SingleInstance();
            builder.RegisterType<InMemoryMessageBusSubscriptionsManager>().As<IMessageBusSubscriptionManager>().SingleInstance();
            builder.RegisterType<SerialMessageBus>().As<IMessageBus>().SingleInstance();
            builder.RegisterType<SagasManager>().As<ISagasManager>().SingleInstance();
            builder.RegisterType<MessageFactory>().As<IMessageFactory>().SingleInstance();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
               // .Enrich.WithMachineName()
               // .Enrich.WithProperty("BPServer", "LocalHub")
                .WriteTo.Console()
               // .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
              //  .WriteTo.Seq("http://localhost:5341/")
                //.WriteTo.Telegram(
                // "981598351:AAEN_nMTBvfi8Wl7rPaZygpv-fXi0F4B8y0",
                //  "383328078")
                .CreateLogger();
            builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
        }
    }
}
