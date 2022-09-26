using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streaming.Client;
using Orleans.Streaming.GrainInterfaces;

await using var client = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "Streaming.Demo";
    })
    .AddSimpleMessageStreamProvider("chat")
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await client.Connect();

var chatRoom = client.GetGrain<IChatRoomGrain>(Guid.NewGuid());

var streamId = await chatRoom.Join("shenjie");
var stream = client.GetStreamProvider("chat").GetStream<ChatMsg>(streamId, "default");

await stream.SubscribeAsync(new ChatRoomObserver());

while (true)
{
    await chatRoom.Send(new ChatMsg("shengjie", "Hello everyone!"));
    
    Console.WriteLine("Press any key to continue.");
    Console.ReadKey();
}
