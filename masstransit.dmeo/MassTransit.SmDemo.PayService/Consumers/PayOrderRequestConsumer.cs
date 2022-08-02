using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.PayService.Consumers;

public class PayOrderRequestConsumer : IConsumer<IPayOrderRequest>
{
    private readonly IRequestClient<IGetOrderStateRequest> _orderRequestClient;
    private readonly ILogger<PayOrderRequestConsumer> _logger;

    public PayOrderRequestConsumer(IRequestClient<IGetOrderStateRequest> orderRequestClient,
        ILogger<PayOrderRequestConsumer> logger)
    {
        _orderRequestClient = orderRequestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPayOrderRequest> context)
    {
        var (state, orderNotFound) =
            await _orderRequestClient.GetResponse<OrderLatestState, OrderNotFound>(new
                { OrderId = context.Message.OrderId });
        if (state.IsCompletedSuccessfully)
        {
            var orderstate = await state;
            var order = orderstate.Message;

            // 标记支付
            if (order.Order.Amount == context.Message.Amount && order.Order.UserId == context.Message.UserId)
            {
                _logger.LogInformation($"Order paid suceed:{order.OrderId}");
                await context.Publish<IOrderPaidEvent>(new { OrderId = context.Message.OrderId });
            }
        }
    }
}