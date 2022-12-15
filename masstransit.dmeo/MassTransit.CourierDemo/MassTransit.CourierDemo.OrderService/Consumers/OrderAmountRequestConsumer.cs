using MassTransit.CourierDemo.OrderService.Repositories;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.OrderService.Consumers;

public class OrderAmountRequestConsumer : IConsumer<IOrderAmountRequest>
{
    public async Task Consume(ConsumeContext<IOrderAmountRequest> context)
    {
        var order = await OrderRepository.Get(context.Message.OrderId);
        await context.RespondAsync<IOrderAmountResponse>(new { order.OrderId, order.Amount });
    }
}