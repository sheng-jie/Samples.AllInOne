using MassTransit.CourierDemo.PaymentService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // services.AddHostedService<Worker>();
        services.AddMassTransitWithRabbitMq();
    })
    .Build();

await host.RunAsync();
