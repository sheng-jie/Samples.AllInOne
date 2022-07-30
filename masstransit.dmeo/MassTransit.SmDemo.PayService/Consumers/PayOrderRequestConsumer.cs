using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.PayService.Consumers;

public class PayOrderRequestConsumer : IConsumer<IPayOrderRequest>
{
    private readonly IRequestClient<IGetOrderRequest> _orderRequestClient;

    public PayOrderRequestConsumer(IRequestClient<IGetOrderRequest> orderRequestClient)
    {
        _orderRequestClient = orderRequestClient;
    }

    public async Task Consume(ConsumeContext<IPayOrderRequest> context)
    {
        var order = await _orderRequestClient.GetResponse<Order>(new { OrderId = context.Message.OrderId });
        // 标记支付
        if (order.Message.Amount == context.Message.Amount)
        {
            await context.Publish<IOrderPaidEvent>(new { OrderId = context.Message.OrderId });
        }
    }
}