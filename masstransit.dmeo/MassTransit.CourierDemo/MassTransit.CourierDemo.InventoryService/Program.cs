using MassTransit;
using MassTransit.CourierDemo.InventoryService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // services.AddHostedService<Worker>();
        services.AddMassTransitWithRabbitMq();
    })
    .Build();

await host.RunAsync();
