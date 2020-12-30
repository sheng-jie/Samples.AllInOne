using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Loki;

namespace Loki.K8s.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog((ctx, cfg) =>
                {
                    var credentials = new NoAuthCredentials("http://localhost:3100");

                    cfg.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);//Microsoft框架本身的日志，仅输出Warning以上级别
                    cfg.Enrich.FromLogContext()
                    .Enrich.WithProperty("App", ctx.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("ENV", ctx.HostingEnvironment.EnvironmentName)
                    .WriteTo.LokiHttp(credentials)
                    .WriteTo.Console();

                    //if (ctx.HostingEnvironment.IsDevelopment())
                    //    cfg.WriteTo.Console(new RenderedCompactJsonFormatter());
                });
    }
}
