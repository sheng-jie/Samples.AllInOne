using Microsoft.Extensions.Hosting;

namespace MassTransit.RequestResponseDemo;

public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var orderRequestClient = _bus.CreateRequestClient<IOrderRequest>(new Uri("queue:orders"));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var orderId = Guid.NewGuid().ToString();
            var response = await orderRequestClient.GetResponse<IOrderResponse>(new OrderRequest(orderId));

            Console.WriteLine(
                $"Get order {orderId} succeed:{response.Message.Order.Amount},{response.Message.Order.PaidTime}");

            await Task.Delay(1000, stoppingToken);
        }
    }
}