using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.PayService.Consumers;

public class PayOrderRequestConsumer : IConsumer<IPayOrderRequest>
{
    private readonly IRequestClient<IGetOrderRequest> _orderRequestClient;
    private readonly ILogger<PayOrderRequestConsumer> _logger;

    public PayOrderRequestConsumer(IRequestClient<IGetOrderRequest> orderRequestClient,ILogger<PayOrderRequestConsumer> logger)
    {
        _orderRequestClient = orderRequestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPayOrderRequest> context)
    {
        var order = await _orderRequestClient.GetResponse<Order>(new { OrderId = context.Message.OrderId });
        // 标记支付
        if (order.Message.Amount == context.Message.Amount)
        {
            _logger.LogInformation($"Order paid suceed:{order.Message.OrderId}");
            await context.Publish<IOrderPaidEvent>(new { OrderId = context.Message.OrderId });
        }
    }
}