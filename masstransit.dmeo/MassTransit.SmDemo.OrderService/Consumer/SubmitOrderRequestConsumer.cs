using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;
using Microsoft.Extensions.Logging;

namespace MassTransit.SmDemo.OrderService.Consumer;

public class SubmitOrderRequestConsumer : IConsumer<ISubmitOrderRequest>
{
    private readonly IRequestClient<IReserveStockRequest> _reserveStockRequestClient;
    private readonly ILogger<SubmitOrderRequestConsumer> _logger;

    public SubmitOrderRequestConsumer(IRequestClient<IReserveStockRequest> reserveStockRequestClient,
        ILogger<SubmitOrderRequestConsumer> logger)
    {
        _reserveStockRequestClient = reserveStockRequestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ISubmitOrderRequest> context)
    {
        _logger.LogWarning($"Recevie order request from user :{context.Message.UserId}");

        //预留库存
        var reserveResponse = await _reserveStockRequestClient.GetResponse<IReserveStockResponse>(
            new
            {
                Items = context.Message.OrderItems.ToDictionary(item => item.GoodId, item => item.Num)
            });

        if (!reserveResponse.Message.IsSucceed)
        {
            await context.RespondAsync<OrderSubmitRejected>(new
            {
                Reason = reserveResponse.Message.Message
            });
            return;
        }

        var newOrder = CreatOrder(context.Message);
        OrderStore.AddOrder(newOrder);

        await context.RespondAsync<OrderSubmitSucceed>(new
        {
            Order = newOrder
        });

        await context.Publish(new OrderSubmittedEvent() { OrderId = newOrder.OrderId });
    }

    private Order CreatOrder(ISubmitOrderRequest orderRequest)
    {
        var newOrder = new Order(Guid.NewGuid().ToString(), orderRequest.UserId, orderRequest.OrderItems);
        return newOrder;
    }
}