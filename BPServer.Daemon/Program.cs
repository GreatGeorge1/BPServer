using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace BPServer.Daemon
{
    public class Program
    {
        static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();
        public static int Main(string[] args)
        {
            bool SQliteLog = true;
            var logBuilder = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .Enrich.WithMachineName()
             .Enrich.WithEnvironmentUserName()
             //.WriteTo.Providers(Providers)
             .WriteTo.Console();
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Linux))
            {
                logBuilder.WriteTo.LocalSyslog();
                //logBuilder.WriteTo.SQLite(@"./Logs/log.db");
            }
            if (SQliteLog==true)
            {
             
                var str = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar.ToString()}Logs{Path.DirectorySeparatorChar.ToString()}logs.db";
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar.ToString()}Logs");
                logBuilder.WriteTo.SQLite(str);
                Log.Logger.Information(str);
            }
            Log.Logger = logBuilder.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions => 
                    {
                        serverOptions.DisableStringReuse = true;
                        serverOptions.Limits.MaxConcurrentConnections = 100;
                        serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                        serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                        serverOptions.Limits.MinRequestBodyDataRate =
                            new MinDataRate(bytesPerSecond: 100,
                                gracePeriod: TimeSpan.FromSeconds(10));
                        serverOptions.Limits.MinResponseDataRate =
                            new MinDataRate(bytesPerSecond: 100,
                                gracePeriod: TimeSpan.FromSeconds(10));
                        serverOptions.Listen(IPAddress.Any, 5000);
                        serverOptions.Listen(IPAddress.Any, 5001,
                            listenOptions =>
                            {
                                listenOptions.UseHttps();
                            });
                        serverOptions.Limits.KeepAliveTimeout =
                            TimeSpan.FromMinutes(2);
                        serverOptions.Limits.RequestHeadersTimeout =
                            TimeSpan.FromMinutes(1);
                        serverOptions.Limits.Http2.MaxStreamsPerConnection = 100;
                        serverOptions.Limits.Http2.HeaderTableSize = 4096;
                    });
                    webBuilder.UseStartup<Startup>();

                })
                .ConfigureLogging(cf =>
                {
                    //cf.AddSerilog();
                })
                .UseSerilog(providers: Providers,logger:Log.Logger);
    }
}
