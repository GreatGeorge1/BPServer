using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BPServer.Autofac;
using BPServer.Core.MessageBus.Command;
using BPServer.Worker.ExternalCommunication.Masstransit;
using BPSever.Infrastracture.MessageTypes;
using ConsoleApp1;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BPServer.Worker
{
    public class Program
    {
        public ILifetimeScope AutofacContainer { get; private set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("BPServer", "LocalHub")
                .WriteTo.Async(a=>a.Console())
                //.WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                //.WriteTo.Seq("http://localhost:5341/")
                //.WriteTo.Telegram(
                // "981598351:AAEN_nMTBvfi8Wl7rPaZygpv-fXi0F4B8y0",
                //  "383328078")
                .CreateLogger();

            var builder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog(Log.Logger)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile(
                        $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddHostedService<Worker>();
                }).ConfigureContainer<ContainerBuilder>(builder =>
                {
                    RegisterBpServer(builder);
                    RegisterCustomMassTransit(builder);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog();
                });

            return builder;
        }

        private static ContainerBuilder RegisterBpServer(ContainerBuilder builder)
        {
            builder.RegisterModule<HubModule>();
            builder.RegisterType<CardNotificationHandler>();
            builder.RegisterType<CardCommand>();
            builder.RegisterType<FingerNotificationHandler>();
            builder.RegisterType<FingerCommand>();
            builder.RegisterType<BleNotificationHandler>();
            builder.RegisterType<BleCommand>();
            builder.RegisterType<PingCommand>();
            builder.RegisterType<PingAcknowledgeHandler>();
            return builder;
        }

        private static ContainerBuilder RegisterCustomMassTransit(ContainerBuilder builder)
        {
            builder.AddMassTransit(x => 
            {
                x.AddConsumer<TestConsumer>();
                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg=> 
                {
                    cfg.Host("localhost", mqcfg=> {
                        mqcfg.Password("guest");
                        mqcfg.Username("guest");

                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.ReceiveEndpoint("test_consumer", ec => 
                    {
                        ec.PrefetchCount = 16;
                        ec.UseMessageRetry(r => r.Immediate(5));
                        ec.ConfigureConsumer<TestConsumer>(context);
                    });
                   
                }));
                x.AddRequestClient<TestRequest>();
            });
            return builder;
        }
    }
}
