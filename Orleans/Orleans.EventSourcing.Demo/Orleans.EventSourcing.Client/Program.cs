using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.EventSourcing.GrainInterfaces;

await using var client = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "EventSourcing.Demo";
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await client.Connect();

var accountA = Guid.Parse("641C9499-0004-4CA7-98A7-29B8C0475E6E");
var accountB = Guid.Parse("70368B7A-7742-41DA-A0EE-BFDBC8DEB4B9");
var a = client.GetGrain<IBankAccountGrain>(accountA);
var b = client.GetGrain<IBankAccountGrain>(accountB);

await a.InitialAccount(1000);
await b.InitialAccount(800);

while (true)
{
    Console.WriteLine(
        $"Before transfer: {accountA}({await a.GetBalanceAsync()}),{accountB}({await b.GetBalanceAsync()})");

    await a.TransferOut(new TransferRequest()
    {
        From = accountA,
        To = accountB,
        Amount = 200,
    });
    
    await b.TransferOut(new TransferRequest()
    {
        From = accountB,
        To = accountA,
        Amount = 100,
    });

    Console.WriteLine(
        $"After transfer: {accountA}({await a.GetBalanceAsync()}),{accountB}({await b.GetBalanceAsync()})");

    Console.WriteLine("Press any key to continue");
    Console.ReadKey();
}