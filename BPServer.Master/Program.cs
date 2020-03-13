using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BPServer.Master
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               //.Enrich.WithMachineName()
               //.Enrich.WithProperty("BPServer", "LocalHub")
               .WriteTo.Async(a => a.Console())
               //.WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
               //.WriteTo.Seq("http://localhost:5341/")
               //.WriteTo.Telegram(
               // "981598351:AAEN_nMTBvfi8Wl7rPaZygpv-fXi0F4B8y0",
               //  "383328078")
               .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog(Log.Logger)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog();
                });
    }
}
