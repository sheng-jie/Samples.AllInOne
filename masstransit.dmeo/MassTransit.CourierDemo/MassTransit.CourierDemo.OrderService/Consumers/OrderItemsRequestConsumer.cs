using MassTransit.CourierDemo.OrderService.Repositories;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.OrderService.Consumers;

public class OrderItemsRequestConsumer : IConsumer<IOrderItemsRequest>
{
    public async Task Consume(ConsumeContext<IOrderItemsRequest> context)
    {
        var order = await OrderRepository.Get(context.Message.OrderId);
        await context.RespondAsync<IOrderItemsResponse>(new
        {
            order.OrderId, 
            DeduceStockItems = order.OrderItems.Select(
                item => new DeduceStockItem(item.SkuId, item.Qty)).ToList()
        });
    }
}