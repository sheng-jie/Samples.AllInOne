using MassTransit.SmDemo.OrderService.Repositories;
using MassTransit.SmDemo.Shared.Contracts;

namespace MassTransit.SmDemo.OrderService.Consumers;

public class CancelOrderConsumer : IConsumer<ICancelOrderCommand>
{
    private readonly ILogger<CancelOrderConsumer> _logger;

    public CancelOrderConsumer(ILogger<CancelOrderConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ICancelOrderCommand> context)
    {
        var order = await OrderRepository.Get(context.Message.OrderId);
        order.CancelOrder();
        _logger.LogWarning(
            $"Order [{order.OrderId} has been canceled!");

        await context.Publish<ICancelOrderSucceed>(new { order.OrderId });
    }
}