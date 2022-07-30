using MassTransit.SmDemo.Share.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransit.SmDemo.OrderService;

public class Worker : BackgroundService
{
    readonly IBus _bus;
    private readonly ILogger<Worker> _logger;

    public Worker(IBus bus, ILogger<Worker> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serviceAddress = "rabbitmq://localhost:5672/orders";
        var orderRequestClient =
            _bus.CreateRequestClient<ISubmitOrderRequest>(new Uri(serviceAddress), TimeSpan.FromSeconds(60));

        while (!stoppingToken.IsCancellationRequested)
        {
            var (accepted, rejected) = await orderRequestClient.GetResponse<OrderSubmitSucceed, OrderSubmitRejected>(
                new
                {
                    UserId = "shengjie",
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem("iphone 14", 1, 5000),
                        new OrderItem("iphone 14 charge", 1, 100)
                    }
                }, stoppingToken);

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                _logger.LogWarning(
                    $"Submit order succeed, orderId:{response.Message.Order.OrderId}, amount:{response.Message.Order.Amount}");
            }
            else
            {
                var response = await rejected;
                _logger.LogWarning(
                    $"Submit order failed:{response.Message.Reason}");
            }

            Console.ReadKey();
        }
    }
}