using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderService.Consumer;

public class GetOrderRequestConsumer:IConsumer<IGetOrderRequest>
{
    public async Task Consume(ConsumeContext<IGetOrderRequest> context)
    {
        await context.RespondAsync<Order>(OrderStore.GetOrder(context.Message.OrderId));
    }
}