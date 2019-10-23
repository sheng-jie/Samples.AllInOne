using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Grains;
using Orleans.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Orleans.Server
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = typeof(Program).FullName;

            // define the cluster configuration

            return Host.CreateDefaultBuilder()
                .UseOrleans((builder) =>
                    {
                        builder.UseLocalhostClustering()
                            .AddMemoryGrainStorageAsDefault()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "HelloWorldApp";
                            })
                            .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                            .ConfigureApplicationParts(parts =>
                                parts.AddApplicationPart(typeof(IPromotionProductManager).Assembly).WithReferences());
                    }
                )
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder => { builder.AddConsole(); })
                .RunConsoleAsync();
        }
    }

}