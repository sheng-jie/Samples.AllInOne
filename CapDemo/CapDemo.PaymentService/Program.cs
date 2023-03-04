using CapDemo.PaymentService;
using CapDemo.PaymentService.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connStr = context.Configuration.GetConnectionString("PaymentDb");
        services.AddCap(x =>
        {
            x.UseMySql(connStr);
            x.UseRabbitMQ("localhost");
        });
        services.AddTransient<PaymentConsumers>();
    })
    .Build();

await host.RunAsync();
