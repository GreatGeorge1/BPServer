using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using BPServer.Master.Services;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BPServer.Master
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<TestRequestConsumer>();
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        cfg.Host("localhost", mqcfg => {
            //            mqcfg.Password("guest");
            //            mqcfg.Username("guest");

            //        });
            //        //cfg.ConfigureEndpoints(provider);
            //        cfg.ReceiveEndpoint("test_request_consumer", ec =>
            //        {
            //            ec.PrefetchCount = 16;
            //            ec.UseMessageRetry(x => x.Interval(2, 100));
            //            ec.ConfigureConsumer<TestRequestConsumer>(provider);
            //           // EndpointConvention.Map<TestRequest>(ec.InputAddress);
            //        });

            //    }));
            //});
            services.AddSingleton<IHostedService, BusService>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<TestRequestConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host("localhost", mqcfg => {
                        mqcfg.Password("guest");
                        mqcfg.Username("guest");

                    });
                    //cfg.ConfigureEndpoints(provider);
                    cfg.ReceiveEndpoint("test_request_consumer", ec =>
                    {
                        ec.PrefetchCount = 16;
                        ec.UseMessageRetry(x => x.Interval(2, 100));
                        ec.ConfigureConsumer<TestRequestConsumer>(provider);
                        // EndpointConvention.Map<TestRequest>(ec.InputAddress);
                    });

                }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
