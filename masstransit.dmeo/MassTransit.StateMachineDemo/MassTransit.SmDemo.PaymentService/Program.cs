using MassTransit;
using MassTransit.SmDemo.PaymentService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransitWithRabbitMq();
        // services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
