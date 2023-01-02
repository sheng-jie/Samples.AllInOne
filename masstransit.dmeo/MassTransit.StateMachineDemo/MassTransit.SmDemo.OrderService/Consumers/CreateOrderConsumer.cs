using MassTransit.SmDemo.OrderService.Domains;
using MassTransit.SmDemo.OrderService.Events;
using MassTransit.SmDemo.OrderService.Repositories;

namespace MassTransit.SmDemo.OrderService.Consumers;

public class CreateOrderConsumer : IConsumer<ICreateOrderCommand>
{
    private readonly ILogger<CreateOrderConsumer> _logger;

    public CreateOrderConsumer(ILogger<CreateOrderConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ICreateOrderCommand> context)
    {
        var shoppingItems =
            context.Message.ShoppingCartItems.Select(item => new ShoppingCartItem(item.SkuId, item.Price, item.Qty));
        var order = new Order(context.Message.CustomerId).NewOrder(shoppingItems.ToArray());
        await OrderRepository.Insert(order);

        
        _logger.LogInformation($"Order {order.OrderId} created successfully");
        await context.Publish<ICreateOrderSucceed>(new
        {
            order.OrderId,
            order.OrderItems
        });
    }
}