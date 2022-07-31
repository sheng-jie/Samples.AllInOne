using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderService.Consumer;

public class GetOrderRequestConsumer:IConsumer<IGetOrderRequest>
{
    public async Task Consume(ConsumeContext<IGetOrderRequest> context)
    {
        var order = OrderStore.GetOrder(context.Message.OrderId);
        await context.RespondAsync<Order>(order);
    }
}