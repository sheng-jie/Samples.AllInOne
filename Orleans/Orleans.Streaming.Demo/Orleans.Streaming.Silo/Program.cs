using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streaming.Grains;

var builder = new HostBuilder().UseOrleans(c =>
{
    c.UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "Streaming.Demo";
        })
        .AddMemoryGrainStorage("PubSubStore")
        .AddSimpleMessageStreamProvider("chat", optionsBuilder =>
        {
            optionsBuilder.FireAndForgetDelivery = true;
        })
        .Configure<EndpointOptions>(
            options => options.AdvertisedIPAddress = IPAddress.Loopback)
        .ConfigureApplicationParts(
            parts => parts.AddApplicationPart(typeof(ChatRoomGrain).Assembly).WithReferences())
        .ConfigureLogging(logging => logging.AddConsole());
});

var host = builder.Build();
await host.RunAsync();