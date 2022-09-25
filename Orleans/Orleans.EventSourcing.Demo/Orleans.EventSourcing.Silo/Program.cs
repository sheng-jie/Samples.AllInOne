using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.EventSourcing.Grains;
using Orleans.Hosting;

var builder = new HostBuilder().UseOrleans(c =>
{
    c.UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "EventSourcing.Demo";
        })
        .AddMemoryGrainStorageAsDefault()
        // .AddLogStorageBasedLogConsistencyProviderAsDefault()
        .AddLogStorageBasedLogConsistencyProvider()
        .Configure<EndpointOptions>(
            options => options.AdvertisedIPAddress = IPAddress.Loopback)
        .ConfigureApplicationParts(
            parts => parts.AddApplicationPart(typeof(BankAccountGrain).Assembly).WithReferences())
        .ConfigureLogging(logging => logging.AddConsole());
});

var host = builder.Build();
await host.RunAsync();