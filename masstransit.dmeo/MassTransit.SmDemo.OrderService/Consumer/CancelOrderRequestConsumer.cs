using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;
using Microsoft.Extensions.Logging;

namespace MassTransit.SmDemo.OrderService.Consumer;

public class CancelOrderRequestConsumer : IConsumer<ICancelOrderRequest>
{
    private readonly ILogger<CancelOrderRequestConsumer> _logger;

    public CancelOrderRequestConsumer(ILogger<CancelOrderRequestConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICancelOrderRequest> context)
    {
        //do some cancel logic here
        //...

        _logger.LogInformation($"Order canceled:{context.Message.OrderId}");

        await context.Publish<IOrderCanceledEvent>(new { OrderId = context.Message.OrderId });
    }
}