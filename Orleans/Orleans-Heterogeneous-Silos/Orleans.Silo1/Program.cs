using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Orleans.Silo1
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            Console.Title = "SiloHost";
            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseAzureStorageClustering(option => option.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=shengjie;AccountKey=IrXu5W6vrvK75qQlFAjbhjrUcwO9GOUWpiMbrJpRbVHnRDeK3Vb0PPzYndG2iuiUdRmi3fvVwzgzqCWaVycFyA==;TableEndpoint=https://shengjie.table.cosmos.azure.com:443/;")
                        //.UseLocalhostClustering(11112, 30001, new IPEndPoint(IPAddress.Loopback, 11111))
                        .Configure<TypeManagementOptions>(option=>option.TypeMapRefreshInterval=TimeSpan.FromSeconds(30))
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "silo1";
                        })
                        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences());
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
