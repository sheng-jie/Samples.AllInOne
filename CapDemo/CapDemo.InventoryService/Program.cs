using CapDemo.InventoryService;
using CapDemo.InventoryService.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connStr = context.Configuration.GetConnectionString("InventoryDb");
        services.AddCap(x =>
        {
            x.UseMySql(connStr);
            x.UseRabbitMQ("localhost");
        });

        services.AddTransient<InventoryConsumer>();
    })
    .Build();

await host.RunAsync();
