using Masstransit.FirstActivity;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddMassTransitWithRabbitMq();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();