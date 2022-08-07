using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit.ScheduleDemo.HangfireServer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHangfire(x => x.UseMemoryStorage());
        services.AddMassTransitWithRabbitMq();
        // services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();