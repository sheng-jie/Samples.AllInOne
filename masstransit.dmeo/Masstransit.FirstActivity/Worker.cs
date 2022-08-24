using MassTransit;
using MassTransit.Courier.Contracts;

namespace Masstransit.FirstActivity;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;

    public Worker(ILogger<Worker> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());
        
        builder.AddActivity("DownloadActivity", new Uri("rabbitmq://localhost/download-image_execute"), new
        {
            ImageUri = new Uri("https://masstransit-project.com/mt-logo-color.png")
        });
        builder.AddActivity("FirstActivity", new Uri("rabbitmq://localhost/first-activity_execute"), new
        {
            Value = "Shengjie"
        });
        
        var routingSlip = builder.Build();

        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Execute(routingSlip);
            _logger.LogInformation("Start routing slip at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);

            Console.ReadKey();
        }
    }
}